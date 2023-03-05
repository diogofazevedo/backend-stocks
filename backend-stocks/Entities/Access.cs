namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Access
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty!;

    [StringLength(100)]
    public string? Description { get; set; }

    [JsonIgnore]
    public int? RoleId { get; set; }
}
