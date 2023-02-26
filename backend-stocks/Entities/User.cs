namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty!;

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty!;

    [Required]
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty!;

    [JsonIgnore]
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public Photo? Photo { get; set; }

    [Required]
    public Role Role { get; set; } = new Role();

    [JsonIgnore]
    public int? RoleId { get; set; }

    [NotMapped]
    public string? ImageUrl { get; set; }
}
