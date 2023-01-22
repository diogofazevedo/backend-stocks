namespace WebApi.Models.Products;

public class ProductUpdateRequest
{
    public string? Name { get; set; }
    public string? CategoryCode { get; set; }
    public IFormFile? File { get; set; }
    public bool LotManagement { get; set; }
    public bool SerialNumberManagement { get; set; }
    public bool LocationManagement { get; set; }
    public string? UnityCode { get; set; }
    public string? User { get; set; }
}
