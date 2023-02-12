namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Categories;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public CategoriesController(
        ICategoryService categoryService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _categoryService = categoryService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = _categoryService.GetAll();
        return Ok(categories);
    }

    [HttpPost]
    public IActionResult Create(CategoryCreateRequest model)
    {
        _categoryService.Create(model);
        return Ok(new { message = "Categoria criada com sucesso." });
    }

    [HttpPut("{code}")]
    public IActionResult Update(string code, CategoryUpdateRequest model)
    {
        _categoryService.Update(code, model);
        return Ok(new { message = "Categoria editada com sucesso." });
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        _categoryService.Delete(code);
        return Ok(new { message = "Categoria eliminada com sucesso." });
    }
}
