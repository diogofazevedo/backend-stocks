namespace WebApi.Models.Products;

using System.ComponentModel.DataAnnotations;

public class ProductCreateRequest
{
    [Required]
    public string Code { get; set; } = string.Empty!;

    [Required]
    public string Name { get; set; } = string.Empty!;

    [Required]
    public string CategoryCode { get; set; } = string.Empty!;
    public IFormFile? File { get; set; }
    public bool LotManagement { get; set; }
    public bool SerialNumberManagement { get; set; }
    public bool LocationManagement { get; set; }

    [Required]
    public string UnityCode { get; set; } = string.Empty!;
    public string User { get; set; } = string.Empty!;
}
