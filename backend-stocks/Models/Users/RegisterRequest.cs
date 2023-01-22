namespace WebApi.Models.Users;

using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    public string Name { get; set; } = string.Empty!;

    [Required]
    public string Username { get; set; } = string.Empty!;

    [Required]
    public string Password { get; set; } = string.Empty!;
    public IFormFile? File { get; set; }

    [Required]
    public int RoleId { get; set; }
}
