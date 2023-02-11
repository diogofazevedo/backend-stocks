namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Unities;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UnitiesController : ControllerBase
{
    private readonly IUnityService _unityService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public UnitiesController(
        IUnityService unityService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _unityService = unityService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var unities = _unityService.GetAll();
        return Ok(unities);
    }

    [HttpGet("{code}")]
    public IActionResult GetByCode(string code)
    {
        var unity = _unityService.GetByCode(code);
        return Ok(unity);
    }

    [HttpPost]
    public IActionResult Create(UnityCreateRequest model)
    {
        _unityService.Create(model);
        return Ok(new { message = "Unidade criada com sucesso." });
    }

    [HttpPut("{code}")]
    public IActionResult Update(string code, UnityUpdateRequest model)
    {
        _unityService.Update(code, model);
        return Ok(new { message = "Unidade editada com sucesso." });
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        _unityService.Delete(code);
        return Ok(new { message = "Unidade eliminada com sucesso." });
    }
}
