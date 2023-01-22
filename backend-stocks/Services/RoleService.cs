namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Roles;
using AutoMapper;

public interface IRoleService
{
    IEnumerable<Role> GetAll();
    Role GetById(int id);
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
        return _context.Roles;
    }

    public Role GetById(int id)
    {
        var role = _context.Roles.Find(id);
        if (role == null) { throw new KeyNotFoundException("Papel não encontrado."); }
        return role;
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

        _context.Roles.Remove(role);
        _context.SaveChanges();
    }
}
