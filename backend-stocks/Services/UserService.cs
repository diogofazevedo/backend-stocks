namespace WebApi.Services;

using BCrypt.Net;
using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using WebApi.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
    AuthenticateResponse RefreshToken(string token, string ipAddress);
    void RevokeToken(string token, string ipAddress);
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

        if (user == null)
        {
            throw new AppException("Utilizador inválido.");
        }

        if (!BCrypt.Verify(model.Password, user.PasswordHash))
        {
            throw new AppException("Palavra-passe inválida.");
        }

        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        user.RefreshTokens?.Add(refreshToken);

        removeOldRefreshTokens(user);

        _context.Update(user);
        _context.SaveChanges();

        var role = _context.Roles.Find(user.RoleId);
        if (role != null)
        {
            var accesses = _context.Accesses.Where(x => x.RoleId == role.Id);
            role.Accesses = accesses.ToList();
            user.Role = role;
        }

        var photo = _context.Photos.SingleOrDefault(x => x.Type == $"USR~{user.Username}");
        if (photo != null)
        {
            string base64String = Convert.ToBase64String(photo.Bytes);
            string imageUrl = $"data:image/{photo.FileExtension.Replace(".", "")};base64,{base64String}";
            user.ImageUrl = imageUrl;
        }

        return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
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

        _context.Update(user);
        _context.SaveChanges();

        var jwtToken = _jwtUtils.GenerateJwtToken(user);

        var role = _context.Roles.Find(user.RoleId);
        if (role != null)
        {
            var accesses = _context.Accesses.Where(x => x.RoleId == role.Id);
            role.Accesses = accesses.ToList();
            user.Role = role;
        }

        var photo = _context.Photos.SingleOrDefault(x => x.Type == $"USR~{user.Username}");
        if (photo != null)
        {
            string base64String = Convert.ToBase64String(photo.Bytes);
            string imageUrl = $"data:image/{photo.FileExtension.Replace(".", "")};base64,{base64String}";
            user.ImageUrl = imageUrl;
        }

        return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users
            .Select(x => new User()
            {
                Id = x.Id,
                Name = x.Name,
                Username = x.Username,
                Photo = x.Photo,
                Role = x.Role,
                ImageUrl = x.Photo != null ? $"data:image/{x.Photo.FileExtension.Replace(".", "")};base64,{Convert.ToBase64String(x.Photo.Bytes)}" : "",
            }).OrderByDescending(x => x.Id);
    }

    public User GetById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) { throw new KeyNotFoundException("Utilizador não encontrado."); }
        return user;
    }

    public void Register(RegisterRequest model)
    {
        if (_context.Users.Any(x => x.Username == model.Username))
        {
            throw new AppException("Utilizador '" + model.Username + "' indisponível.");
        }

        var user = _mapper.Map<User>(model);

        if (model.File != null)
        {
            using var memoryStream = new MemoryStream();
            model.File.CopyTo(memoryStream);

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
        if (role == null) { throw new KeyNotFoundException("Papel '" + model.RoleId + "' não encontrado."); }
        user.Role = role;

        user.PasswordHash = BCrypt.HashPassword(model.Password);

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void RevokeToken(string token, string ipAddress)
    {
        var user = getUserByRefreshToken(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
        {
            throw new AppException("Token inválido.");
        }

        revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
        _context.Update(user);
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
        if (role == null) { throw new KeyNotFoundException("Papel '" + model.RoleId + "' não encontrado."); }
        user.Role = role;

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

    private void revokeRefreshToken(RefreshToken token, string ipAddress, string? reason = "", string? replacedByToken = "")
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }
}
