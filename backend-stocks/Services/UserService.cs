namespace WebApi.Services;

using BCrypt.Net;
using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Http;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
    AuthenticateResponse RefreshToken(string token, string ipAddress);
    IEnumerable<User> GetAll();
    User GetById(int id);
    void Register(RegisterRequest model);
    void Update(int id, UpdateRequest model);
    void Delete(int id);
}

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
    {
        var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);

        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
        {
            throw new AppException("Utilizador ou palavra-passe inválida.");
        }

        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        user.RefreshTokens?.Add(refreshToken);

        removeOldRefreshTokens(user);

        byte[] bytes = Array.Empty<byte>();
        IFormFile file = new FormFile(new MemoryStream(bytes), 0, 0, "", "");

        if (user.Photo != null)
        {
            using var stream = new MemoryStream(user.Photo.Bytes);
            file = new FormFile(stream, 0, user.Photo.Bytes.LongLength, user.Photo.Description, user.Photo.Description);
        }

        _context.Update(user);
        _context.SaveChanges();

        return new AuthenticateResponse(user, jwtToken, refreshToken.Token, file, user.Role);
    }

    public AuthenticateResponse RefreshToken(string token, string ipAddress)
    {
        var user = getUserByRefreshToken(token);
        var refreshToken = user.RefreshTokens?.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
            _context.Update(user);
            _context.SaveChanges();
        }

        if (!refreshToken.IsActive) { throw new AppException("Token inválido."); }

        var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
        user.RefreshTokens?.Add(newRefreshToken);

        removeOldRefreshTokens(user);

        byte[] bytes = Array.Empty<byte>();
        IFormFile file = new FormFile(new MemoryStream(bytes), 0, 0, "", "");

        if (user.Photo != null)
        {
            using var stream = new MemoryStream(user.Photo.Bytes);
            file = new FormFile(stream, 0, user.Photo.Bytes.LongLength, user.Photo.Description, user.Photo.Description);
        }

        _context.Update(user);
        _context.SaveChanges();

        var jwtToken = _jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token, file, user.Role);
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) { throw new KeyNotFoundException("Utilizador não encontrado."); }
        return user;
    }

    public async void Register(RegisterRequest model)
    {
        if (_context.Users.Any(x => x.Username == model.Username))
        {
            throw new AppException("Utilizador '" + model.Username + "' indisponível.");
        }

        var user = _mapper.Map<User>(model);

        if (model.File != null)
        {
            using var memoryStream = new MemoryStream();
            await model.File.CopyToAsync(memoryStream);

            if (memoryStream.Length < 2097152)
            {
                var newPhoto = new Photo()
                {
                    Bytes = memoryStream.ToArray(),
                    Description = model.File.FileName,
                    FileExtension = Path.GetExtension(model.File.FileName),
                    Size = model.File.Length,
                    Type = $"USR~{model.Username}"
                };
                user.Photo = newPhoto;
            }
            else
            {
                throw new AppException("Imagem inválida (> 2 MB).");
            }
        }

        var role = _context.Roles.Find(model.RoleId);
        if (role != null)
        {
            user.Role = role;
        }

        user.PasswordHash = BCrypt.HashPassword(model.Password);

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public async void Update(int id, UpdateRequest model)
    {
        var user = _context.Users.Find(id);
        if (user == null) { throw new KeyNotFoundException("Utilizador não encontrado."); }

        if (model.Username != user.Username && _context.Users.Any(x => x.Username == model.Username))
        {
            throw new AppException("Utilizador '" + model.Username + "' indisponível.");
        }

        if (!BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
        {
            throw new KeyNotFoundException("Palavra-passe atual incorreta.");
        }

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            user.PasswordHash = BCrypt.HashPassword(model.NewPassword);
        }

        if (model.File != null)
        {
            using var memoryStream = new MemoryStream();
            await model.File.CopyToAsync(memoryStream);

            if (memoryStream.Length < 2097152)
            {
                var newPhoto = new Photo()
                {
                    Bytes = memoryStream.ToArray(),
                    Description = model.File.FileName,
                    FileExtension = Path.GetExtension(model.File.FileName),
                    Size = model.File.Length,
                    Type = $"USR~{model.Username}"
                };
                user.Photo = newPhoto;
            }
            else
            {
                throw new AppException("Imagem inválida (> 2 MB).");
            }
        }

        var role = _context.Roles.Find(model.RoleId);
        if (role != null)
        {
            user.Role = role;
        }

        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) { throw new KeyNotFoundException("Utilizador não encontrado."); }

        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    private User getUserByRefreshToken(string token)
    {
        var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        if (user == null) { throw new AppException("Token inválido."); }
        return user;
    }

    private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    private void removeOldRefreshTokens(User user)
    {
        user.RefreshTokens?.RemoveAll(x => !x.IsActive && x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private void revokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
    {
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = user.RefreshTokens?.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken.IsActive)
            {
                revokeRefreshToken(childToken, ipAddress, reason);
            }
            else
            {
                revokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }
    }

    private void revokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }
}
