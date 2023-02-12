namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.StockTransactions;
using AutoMapper;

public interface IStockTransactionService
{
    IEnumerable<StockTransaction> GetAll(string? type);
    void Create(StockTransactionCreateRequest model);
    void Update(int id, StockTransactionUpdateRequest model);
}

public class StockTransactionService : IStockTransactionService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public StockTransactionService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<StockTransaction> GetAll(string? type)
    {
        List<StockTransaction> stockTransactions = new();

        if (type == "ENT")
        {
            stockTransactions = _context.StockTransactions.Where(x => x.Quantity > 0).ToList();
        }
        else
        {
            stockTransactions = _context.StockTransactions.Where(x => x.Quantity < 0).ToList();
        }

        return stockTransactions.Select(x => new StockTransaction()
        {
            Id = x.Id,
            Product = x.Product,
            Quantity = x.Quantity,
            Unity = x.Unity,
            Lot = x.Lot,
            SerialNumber = x.SerialNumber,
            Location = x.Location,
            Observation = x.Observation,
        });
    }

    public void Create(StockTransactionCreateRequest model)
    {
        var product = _context.Products.Find(model.ProductCode);
        var unity = _context.Unities.Find(model.UnityCode);
        var location = _context.Locations.Find(model.LocationCode);

        if (model.StockId == null)
        {
            var stockLine = _context.Stock.SingleOrDefault(
                x => x.Product.Code == model.ProductCode &&
                     x.Unity.Code == model.UnityCode &&
                     x.Lot == model.Lot &&
                     x.SerialNumber == model.SerialNumber &&
                     x.Location.Code == model.LocationCode
                );

            if (stockLine == null)
            {
                Stock newStockLine = new()
                {
                    Product = product,
                    Unity = unity,
                    Quantity = model.Quantity,
                    Lot = model.Lot,
                    SerialNumber = model.SerialNumber,
                    Location = location,
                };

                _context.Stock.Add(newStockLine);
            }
            else
            {
                stockLine.Quantity += model.Quantity;
                _context.Stock.Update(stockLine);
            }
        }
        else
        {
            var stockLine = _context.Stock.Find(model.StockId);

            if (stockLine == null)
            {
                throw new AppException("Stock não encontrado.");
            }

            switch (stockLine.Quantity - model.Quantity)
            {
                case < 0:
                    throw new AppException("Quantidade a tratar superior à quantidade em stock.");
                case 0:
                    _context.Stock.Remove(stockLine);
                    break;
                case > 0:
                    stockLine.Quantity -= model.Quantity;
                    _context.Stock.Update(stockLine);
                    break;
                default:
            }
        }

        StockTransaction stockTransaction = new()
        {
            Product = product,
            Quantity = model.StockId == null ? model.Quantity : -model.Quantity,
            Unity = unity,
            Lot = model.Lot,
            SerialNumber = model.SerialNumber,
            Location = location,
            Observation = model.Observation,
            Created = DateTime.UtcNow,
            CreatedBy = model.User,
            Updated = DateTime.UtcNow,
            UpdatedBy = model.User,
        };

        _context.StockTransactions.Add(stockTransaction);
        _context.SaveChanges();
    }

    public void Update(int id, StockTransactionUpdateRequest model)
    {
        var stockTransaction = _context.StockTransactions.Find(id);
        if (stockTransaction == null) { throw new KeyNotFoundException("Movimento não encontrado."); }

        stockTransaction.Updated = DateTime.UtcNow;
        stockTransaction.UpdatedBy = model.User;

        _mapper.Map(model, stockTransaction);
        _context.StockTransactions.Update(stockTransaction);
        _context.SaveChanges();
    }
}
