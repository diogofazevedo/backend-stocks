namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty!;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty!;
    public DateTime Created { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }
    public DateTime Updated { get; set; }

    [StringLength(50)]
    public string? UpdatedBy { get; set; }
}
