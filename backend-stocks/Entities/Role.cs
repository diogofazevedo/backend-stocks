namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty!;

    [Required]
    public List<Access> Accesses { get; set; } = new List<Access>();

    [JsonIgnore]
    public DateTime Created { get; set; }

    [JsonIgnore]
    [StringLength(50)]
    public string? CreatedBy { get; set; }

    [JsonIgnore]
    public DateTime Updated { get; set; }

    [JsonIgnore]
    [StringLength(50)]
    public string? UpdatedBy { get; set; }
}
