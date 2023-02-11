namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Locations;
using AutoMapper;

public interface ILocationService
{
    IEnumerable<Location> GetAll();
    Location GetByCode(string code);
    void Create(LocationCreateRequest model);
    void Update(string code, LocationUpdateRequest model);
    void Delete(string code);
}

public class LocationService : ILocationService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public LocationService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<Location> GetAll()
    {
        return _context.Locations;
    }

    public Location GetByCode(string code)
    {
        var location = _context.Locations.Find(code);
        if (location == null) { throw new KeyNotFoundException("Localização não encontrada."); }
        return location;
    }

    public void Create(LocationCreateRequest model)
    {
        if (_context.Locations.Any(x => x.Code == model.Code))
        {
            throw new AppException("Localização '" + model.Code + "' indisponível.");
        }

        var location = _mapper.Map<Location>(model);

        location.Created = DateTime.UtcNow;
        location.CreatedBy = model.User;

        location.Updated = DateTime.UtcNow;
        location.UpdatedBy = model.User;

        _context.Locations.Add(location);
        _context.SaveChanges();
    }

    public void Update(string code, LocationUpdateRequest model)
    {
        var location = _context.Locations.Find(code);
        if (location == null) { throw new KeyNotFoundException("Localização não encontrada."); }

        location.Updated = DateTime.UtcNow;
        location.UpdatedBy = model.User;

        _mapper.Map(model, location);
        _context.Locations.Update(location);
        _context.SaveChanges();
    }

    public void Delete(string code)
    {
        var location = _context.Locations.Find(code);
        if (location == null) { throw new KeyNotFoundException("Localização não encontrada."); }

        if (_context.Stock.Any(x => x.Location.Code == code))
        {
            throw new AppException("Elimine o stock associado a esta localização antes de eliminar.");
        }

        _context.Locations.Remove(location);
        _context.SaveChanges();
    }
}
