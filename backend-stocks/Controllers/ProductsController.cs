namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Products;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public ProductsController(
        IProductService productService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _productService = productService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var products = _productService.GetAll();
        return Ok(products);
    }

    [HttpPost]
    public IActionResult Create(ProductCreateRequest model)
    {
        _productService.Create(model);
        return Ok(new { message = "Artigo criado com sucesso." });
    }

    [HttpPut("{code}")]
    public IActionResult Update(string code, ProductUpdateRequest model)
    {
        _productService.Update(code, model);
        return Ok(new { message = "Artigo editado com sucesso." });
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        _productService.Delete(code);
        return Ok(new { message = "Artigo eliminado com sucesso." });
    }
}
