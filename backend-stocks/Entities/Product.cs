namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Product
{
    [Key]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty!;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty!;

    [Required]
    public Category Category { get; set; } = new Category();
    public Photo? Photo { get; set; }

    [NotMapped]
    public string? ImageUrl { get; set; }
    public bool LotManagement { get; set; }
    public bool SerialNumberManagement { get; set; }
    public bool LocationManagement { get; set; }

    [Required]
    public Unity StockUnity { get; set; } = new Unity();

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
