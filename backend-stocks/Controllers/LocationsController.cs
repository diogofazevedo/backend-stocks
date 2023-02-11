namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Locations;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public LocationsController(
        ILocationService locationService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _locationService = locationService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var locations = _locationService.GetAll();
        return Ok(locations);
    }

    [HttpGet("{code}")]
    public IActionResult GetByCode(string code)
    {
        var location = _locationService.GetByCode(code);
        return Ok(location);
    }

    [HttpPost]
    public IActionResult Create(LocationCreateRequest model)
    {
        _locationService.Create(model);
        return Ok(new { message = "Localização criada com sucesso." });
    }

    [HttpPut("{code}")]
    public IActionResult Update(string code, LocationUpdateRequest model)
    {
        _locationService.Update(code, model);
        return Ok(new { message = "Localização editada com sucesso." });
    }

    [HttpDelete("{code}")]
    public IActionResult Delete(string code)
    {
        _locationService.Delete(code);
        return Ok(new { message = "Localização eliminada com sucesso." });
    }
}
