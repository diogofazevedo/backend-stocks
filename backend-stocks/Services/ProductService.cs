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
                ImageUrl = x.Photo != null ? $"data:image/{x.Photo.FileExtension.Replace(".", "")};base64,{Convert.ToBase64String(x.Photo.Bytes)}" : "",
                LotManagement = x.LotManagement,
                SerialNumberManagement = x.SerialNumberManagement,
                LocationManagement = x.LocationManagement,
                StockUnity = x.StockUnity,
                Created = x.Created,
            }).OrderByDescending(x => x.Created);
    }

    public void Create(ProductCreateRequest model)
    {
        if (_context.Products.Any(x => x.Code == model.Code))
        {
            throw new AppException("Artigo '" + model.Code + "' indispon�vel.");
        }

        var product = _mapper.Map<Product>(model);

        var category = _context.Categories.Find(model.CategoryCode);
        if (category == null) { throw new KeyNotFoundException("Categoria '" + model.CategoryCode + "' n�o encontrada."); }
        product.Category = category;

        if (model.File != null)
        {
            using var memoryStream = new MemoryStream();
            model.File.CopyToAsync(memoryStream);

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
                throw new AppException("Imagem inv�lida (> 2 MB).");
            }
        }

        var unity = _context.Unities.Find(model.UnityCode);
        if (unity == null) { throw new KeyNotFoundException("Unidade '" + model.UnityCode + "' n�o encontrada."); }
        product.StockUnity = unity;

        product.Created = DateTime.UtcNow;
        product.CreatedBy = model.User;

        product.Updated = DateTime.UtcNow;
        product.UpdatedBy = model.User;

        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Update(string code, ProductUpdateRequest model)
    {
        var product = _context.Products.Find(code);
        if (product == null) { throw new KeyNotFoundException("Artigo n�o encontrado."); }

        var category = _context.Categories.Find(model.CategoryCode);
        if (category == null) { throw new KeyNotFoundException("Categoria '" + model.CategoryCode + "' n�o encontrada."); }
        product.Category = category;

        if (model.File != null)
        {
            var photo = _context.Photos.SingleOrDefault(x => x.Type == $"PRD~{product.Code}");
            if (photo != null)
            {
                _context.Photos.Remove(photo);
            }

            using var memoryStream = new MemoryStream();
            model.File.CopyToAsync(memoryStream);

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
                throw new AppException("Imagem inv�lida (> 2 MB).");
            }
        }

        var unity = _context.Unities.Find(model.UnityCode);
        if (unity == null) { throw new KeyNotFoundException("Unidade '" + model.UnityCode + "' n�o encontrada."); }
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
        if (product == null) { throw new KeyNotFoundException("Artigo n�o encontrado."); }

        if (_context.StockTransactions.Any(x => x.Product.Code == code))
        {
            throw new AppException("N�o � poss�vel eliminar este artigo (movimentos de stock associados).");
        }

        var photo = _context.Photos.SingleOrDefault(x => x.Type == $"PRD~{product.Code}");
        if (photo != null)
        {
            _context.Photos.Remove(photo);
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}
