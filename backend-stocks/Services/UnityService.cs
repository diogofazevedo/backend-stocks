namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Unities;
using AutoMapper;

public interface IUnityService
{
    IEnumerable<Unity> GetAll();
    Unity GetByCode(string code);
    void Create(UnityCreateRequest model);
    void Update(string code, UnityUpdateRequest model);
    void Delete(string code);
}

public class UnityService : IUnityService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public UnityService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<Unity> GetAll()
    {
        return _context.Unities;
    }

    public Unity GetByCode(string code)
    {
        var unity = _context.Unities.Find(code);
        if (unity == null) { throw new KeyNotFoundException("Unidade não encontrada."); }
        return unity;
    }

    public void Create(UnityCreateRequest model)
    {
        if (_context.Unities.Any(x => x.Code == model.Code))
        {
            throw new AppException("Unidade '" + model.Code + "' indisponível.");
        }

        var unity = _mapper.Map<Unity>(model);

        unity.Created = DateTime.UtcNow;
        unity.CreatedBy = model.User;

        unity.Updated = DateTime.UtcNow;
        unity.UpdatedBy = model.User;

        _context.Unities.Add(unity);
        _context.SaveChanges();
    }

    public void Update(string code, UnityUpdateRequest model)
    {
        var unity = _context.Unities.Find(code);
        if (unity == null) { throw new KeyNotFoundException("Unidade não encontrada."); }

        unity.Updated = DateTime.UtcNow;
        unity.UpdatedBy = model.User;

        _mapper.Map(model, unity);
        _context.Unities.Update(unity);
        _context.SaveChanges();
    }

    public void Delete(string code)
    {
        var unity = _context.Unities.Find(code);
        if (unity == null) { throw new KeyNotFoundException("Unidade não encontrada."); }

        if (_context.Products.Any(x => x.StockUnity.Code == code))
        {
            throw new AppException("Desassocie esta unidade dos artigos antes de eliminar.");
        }

        _context.Unities.Remove(unity);
        _context.SaveChanges();
    }
}
