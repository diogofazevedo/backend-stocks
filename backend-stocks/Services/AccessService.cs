namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using AutoMapper;

public interface IAccessService
{
    IEnumerable<Access> GetAll();
}

public class AccessService : IAccessService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public AccessService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<Access> GetAll()
    {
        return _context.Accesses;
    }
}
