namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using AutoMapper;

public interface IStockService
{
    IEnumerable<Stock> GetAllByProduct(string product);
}

public class StockService : IStockService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public StockService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<Stock> GetAllByProduct(string product)
    {
        return _context.Stock
            .Where(x => x.Product.Code == product)
            .Select(x => new Stock()
            {
                Id = x.Id,
                Quantity = x.Quantity,
                Unity = x.Unity,
                Lot = x.Lot,
                SerialNumber = x.SerialNumber,
                Location = x.Location,
            });
    }
}
