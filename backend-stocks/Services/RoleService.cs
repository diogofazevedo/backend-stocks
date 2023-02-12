namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Roles;
using AutoMapper;

public interface IRoleService
{
    IEnumerable<Role> GetAll();
    void Create(RoleCreateRequest model);
    void Update(int id, RoleUpdateRequest model);
    void Delete(int id);
}

public class RoleService : IRoleService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public RoleService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<Role> GetAll()
    {
        return _context.Roles
            .Select(x => new Role()
            {
                Id = x.Id,
                Name = x.Name,
                Accesses = x.Accesses,
            });
    }

    public void Create(RoleCreateRequest model)
    {
        var role = _mapper.Map<Role>(model);

        role.Created = DateTime.UtcNow;
        role.CreatedBy = model.User;

        role.Updated = DateTime.UtcNow;
        role.UpdatedBy = model.User;

        _context.Roles.Add(role);
        _context.SaveChanges();
    }

    public void Update(int id, RoleUpdateRequest model)
    {
        var role = _context.Roles.Find(id);
        if (role == null) { throw new KeyNotFoundException("Papel não encontrado."); }

        var accesses = _context.Accesses.Where(x => x.RoleId == id);
        if (accesses != null)
        {
            _context.Accesses.RemoveRange(accesses);
        }

        role.Updated = DateTime.UtcNow;
        role.UpdatedBy = model.User;

        _mapper.Map(model, role);
        _context.Roles.Update(role);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var role = _context.Roles.Find(id);
        if (role == null) { throw new KeyNotFoundException("Papel não encontrado."); }

        if (_context.Users.Any(x => x.Role.Id == id))
        {
            throw new AppException("Desassocie este papel dos utilizadores antes de eliminar.");
        }

        var accesses = _context.Accesses.Where(x => x.RoleId == role.Id);
        if (accesses != null)
        {
            _context.Accesses.RemoveRange(accesses);
        }

        _context.Roles.Remove(role);
        _context.SaveChanges();
    }
}
