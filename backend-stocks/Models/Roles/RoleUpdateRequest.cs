namespace WebApi.Models.Roles;

using WebApi.Entities;

public class RoleUpdateRequest
{
    public string? Name { get; set; }
    public List<Access>? Accesses { get; set; }
    public string? User { get; set; }
}
