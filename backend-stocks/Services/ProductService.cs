namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Products;
using AutoMapper;

public interface IProductService
{
    IEnumerable<Product> GetAll();
    void Create(ProductCreateRequest model);
    void Update(string code, ProductUpdateRequest model);
    void Delete(string code);
}

public class ProductService : IProductService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public ProductService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<Product> GetAll()
    {
        return _context.Products
            .Select(x => new Product()
            {
                Code = x.Code,
                Name = x.Name,
                Category = x.Category,
                Photo = x.Photo,
                LotManagement = x.LotManagement,
                SerialNumberManagement = x.SerialNumberManagement,
                LocationManagement = x.LocationManagement,
                StockUnity = x.StockUnity,
            });
    }

    public async void Create(ProductCreateRequest model)
    {
        if (_context.Products.Any(x => x.Code == model.Code))
        {
            throw new AppException("Artigo '" + model.Code + "' indisponível.");
        }

        var product = _mapper.Map<Product>(model);

        var category = _context.Categories.Find(model.CategoryCode);
        if (category == null) { throw new KeyNotFoundException("Categoria '" + model.CategoryCode + "' não encontrada."); }
        product.Category = category;

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
                    Type = $"PRD~{model.Code}"
                };
                product.Photo = newPhoto;
            }
            else
            {
                throw new AppException("Imagem inválida (> 2 MB).");
            }
        }

        var unity = _context.Unities.Find(model.UnityCode);
        if (unity == null) { throw new KeyNotFoundException("Unidade '" + model.UnityCode + "' não encontrada."); }
        product.StockUnity = unity;

        product.Created = DateTime.UtcNow;
        product.CreatedBy = model.User;

        product.Updated = DateTime.UtcNow;
        product.UpdatedBy = model.User;

        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public async void Update(string code, ProductUpdateRequest model)
    {
        var product = _context.Products.Find(code);
        if (product == null) { throw new KeyNotFoundException("Artigo não encontrado."); }

        var category = _context.Categories.Find(model.CategoryCode);
        if (category == null) { throw new KeyNotFoundException("Categoria '" + model.CategoryCode + "' não encontrada."); }
        product.Category = category;

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
                    Type = $"PRD~{code}"
                };
                product.Photo = newPhoto;
            }
            else
            {
                throw new AppException("Imagem inválida (> 2 MB).");
            }
        }

        var unity = _context.Unities.Find(model.UnityCode);
        if (unity == null) { throw new KeyNotFoundException("Unidade '" + model.UnityCode + "' não encontrada."); }
        product.StockUnity = unity;

        product.Updated = DateTime.UtcNow;
        product.UpdatedBy = model.User;

        _mapper.Map(model, product);
        _context.Products.Update(product);
        _context.SaveChanges();
    }

    public void Delete(string code)
    {
        var product = _context.Products.Find(code);
        if (product == null) { throw new KeyNotFoundException("Artigo não encontrado."); }

        if (_context.StockTransactions.Any(x => x.Product.Code == code))
        {
            throw new AppException("Remova os movimentos de stock deste artigo antes de eliminar.");
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}
