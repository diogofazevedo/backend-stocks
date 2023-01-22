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
public class AccessesController : ControllerBase
{
    private readonly IAccessService _accessService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public AccessesController(
        IAccessService accessService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _accessService = accessService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var accesses = _accessService.GetAll();
        return Ok(accesses);
    }
}
