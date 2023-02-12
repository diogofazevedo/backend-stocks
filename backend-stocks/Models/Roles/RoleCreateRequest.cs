namespace WebApi.Models.Roles;

using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

public class RoleCreateRequest
{
    [Required]
    public string Name { get; set; } = string.Empty!;

    [Required]
    public List<Access> Accesses { get; set; } = new List<Access>();
    public string User { get; set; } = string.Empty!;
}
