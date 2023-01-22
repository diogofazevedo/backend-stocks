namespace WebApi.Models.Users;

using System.Text.Json.Serialization;
using WebApi.Entities;

public class AuthenticateResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string JwtToken { get; set; }

    [JsonIgnore]
    public string RefreshToken { get; set; }

    public IFormFile File { get; set; }
    public Role Role { get; set; }

    public AuthenticateResponse(User user, string jwtToken, string refreshToken, IFormFile file, Role role)
    {
        Id = user.Id;
        Name = user.Name;
        Username = user.Username;
        JwtToken = jwtToken;
        RefreshToken = refreshToken;
        File = file;
        Role = role;
    }
}
