namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Category
{
    [Key]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty!;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty!;

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
