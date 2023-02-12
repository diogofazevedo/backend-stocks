namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public StockController(
        IStockService stockService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _stockService = stockService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll(string product)
    {
        var stock = _stockService.GetAll(product);
        return Ok(stock);
    }
}
