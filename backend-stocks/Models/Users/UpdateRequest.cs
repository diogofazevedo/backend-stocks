namespace WebApi.Models.Users;

public class UpdateRequest
{
    public string? Name { get; set; }
    public string? Username { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public IFormFile? File { get; set; }
    public int RoleId { get; set; }
}
