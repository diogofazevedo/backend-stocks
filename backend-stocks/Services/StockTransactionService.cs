namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.StockTransactions;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

public interface IStockTransactionService
{
    IEnumerable<StockTransaction> GetAll();
    IEnumerable<StockTransaction> GetAllByType(string type);
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

    public IEnumerable<StockTransaction> GetAll()
    {
        return _context.StockTransactions.Where(x => x.Created >= DateTime.UtcNow.AddDays(-7)).Select(x => new StockTransaction()
        {
            Id = x.Id,
            Product = x.Product,
            Quantity = x.Quantity,
            Unity = x.Unity,
            Lot = x.Lot,
            SerialNumber = x.SerialNumber,
            Location = x.Location,
            Observation = x.Observation,
            Created = x.Created,
        }).OrderByDescending(x => x.Created);
    }

    public IEnumerable<StockTransaction> GetAllByType(string type)
    {
        if (type == "ENT")
        {
            return _context.StockTransactions.Where(x => x.Quantity > 0).Select(x => new StockTransaction()
            {
                Id = x.Id,
                Product = x.Product,
                Quantity = x.Quantity,
                Unity = x.Unity,
                Lot = x.Lot,
                SerialNumber = x.SerialNumber,
                Location = x.Location,
                Observation = x.Observation,
                Created = x.Created,
            }).OrderByDescending(x => x.Created);
        }

        return _context.StockTransactions.Where(x => x.Quantity < 0).Select(x => new StockTransaction()
        {
            Id = x.Id,
            Product = x.Product,
            Quantity = -x.Quantity,
            Unity = x.Unity,
            Lot = x.Lot,
            SerialNumber = x.SerialNumber,
            Location = x.Location,
            Observation = x.Observation,
            Created = x.Created
        }).OrderByDescending(x => x.Created);
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
            else if (stockLine.SerialNumber.IsNullOrEmpty())
            {
                stockLine.Quantity += model.Quantity;
                _context.Stock.Update(stockLine);
            }
            else
            {
                throw new AppException("N�mero de s�rie j� existente para este artigo.");
            }
        }
        else
        {
            var stockLine = _context.Stock.Find(model.StockId) ?? throw new AppException("Stock n�o encontrado.");
            switch (stockLine.Quantity - model.Quantity)
            {
                case < 0:
                    throw new AppException("Quantidade a tratar superior � quantidade em stock.");
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
        if (stockTransaction == null) { throw new KeyNotFoundException("Movimento n�o encontrado."); }

        stockTransaction.Updated = DateTime.UtcNow;
        stockTransaction.UpdatedBy = model.User;

        _mapper.Map(model, stockTransaction);
        _context.StockTransactions.Update(stockTransaction);
        _context.SaveChanges();
    }
}
