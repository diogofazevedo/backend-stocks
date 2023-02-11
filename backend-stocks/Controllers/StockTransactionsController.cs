namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.StockTransactions;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StockTransactionsController : ControllerBase
{
    private readonly IStockTransactionService _stockTransactionService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public StockTransactionsController(
        IStockTransactionService stockTransactionService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _stockTransactionService = stockTransactionService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll(string type)
    {
        var stockTransactions = _stockTransactionService.GetAll(type);
        return Ok(stockTransactions);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var stockTransaction = _stockTransactionService.GetById(id);
        return Ok(stockTransaction);
    }

    [HttpPost]
    public IActionResult Create(StockTransactionCreateRequest model)
    {
        _stockTransactionService.Create(model);
        return Ok(new { message = "Transação criada com sucesso." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, StockTransactionUpdateRequest model)
    {
        _stockTransactionService.Update(id, model);
        return Ok(new { message = "Transação editada com sucesso." });
    }
}
