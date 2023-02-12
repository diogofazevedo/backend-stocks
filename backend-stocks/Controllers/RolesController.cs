namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Roles;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public RolesController(
        IRoleService roleService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _roleService = roleService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var roles = _roleService.GetAll();
        return Ok(roles);
    }

    [HttpPost]
    public IActionResult Create(RoleCreateRequest model)
    {
        _roleService.Create(model);
        return Ok(new { message = "Papel criado com sucesso." });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, RoleUpdateRequest model)
    {
        _roleService.Update(id, model);
        return Ok(new { message = "Papel editado com sucesso." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _roleService.Delete(id);
        return Ok(new { message = "Papel eliminado com sucesso." });
    }
}
