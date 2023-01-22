namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;

public class Location
{
    [Key]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty!;

    [StringLength(100)]
    public string? Description { get; set; }
    public DateTime Created { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }
    public DateTime Updated { get; set; }

    [StringLength(50)]
    public string? UpdatedBy { get; set; }
}
