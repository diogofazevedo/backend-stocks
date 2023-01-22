namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;

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
    public bool LotManagement { get; set; }
    public bool SerialNumberManagement { get; set; }
    public bool LocationManagement { get; set; }

    [Required]
    public Unity StockUnity { get; set; } = new Unity();
    public DateTime Created { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }
    public DateTime Updated { get; set; }

    [StringLength(50)]
    public string? UpdatedBy { get; set; }
}
