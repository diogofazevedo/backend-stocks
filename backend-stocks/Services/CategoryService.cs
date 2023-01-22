namespace WebApi.Services;

using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Categories;
using AutoMapper;

public interface ICategoryService
{
    IEnumerable<Category> GetAll();
    Category GetByCode(string code);
    void Create(CategoryCreateRequest model);
    void Update(string code, CategoryUpdateRequest model);
    void Delete(string code);
}

public class CategoryService : ICategoryService
{
    private readonly DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public CategoryService(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IMapper mapper)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _mapper = mapper;
    }

    public IEnumerable<Category> GetAll()
    {
        return _context.Categories;
    }

    public Category GetByCode(string code)
    {
        var category = _context.Categories.Find(code);
        if (category == null) { throw new KeyNotFoundException("Categoria não encontrada."); }
        return category;
    }

    public void Create(CategoryCreateRequest model)
    {
        if (_context.Categories.Any(x => x.Code == model.Code))
        {
            throw new AppException("Categoria '" + model.Code + "' indisponível.");
        }

        var category = _mapper.Map<Category>(model);

        category.Created = DateTime.UtcNow;
        category.CreatedBy = model.User;

        category.Updated = DateTime.UtcNow;
        category.UpdatedBy = model.User;

        _context.Categories.Add(category);
        _context.SaveChanges();
    }

    public void Update(string code, CategoryUpdateRequest model)
    {
        var category = _context.Categories.Find(code);
        if (category == null) { throw new KeyNotFoundException("Categoria não encontrada."); }

        category.Updated = DateTime.UtcNow;
        category.UpdatedBy = model.User;

        _mapper.Map(model, category);
        _context.Categories.Update(category);
        _context.SaveChanges();
    }

    public void Delete(string code)
    {
        var category = _context.Categories.Find(code);
        if (category == null) { throw new KeyNotFoundException("Categoria não encontrada."); }

        if (_context.Products.Any(x => x.Category.Code == code))
        {
            throw new AppException("Desassocie esta categoria dos artigos antes de eliminar.");
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();
    }
}
