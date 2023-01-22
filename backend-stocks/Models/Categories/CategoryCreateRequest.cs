namespace WebApi.Models.Categories;

using System.ComponentModel.DataAnnotations;

public class CategoryCreateRequest
{
    [Required]
    public string Code { get; set; } = string.Empty!;

    [Required]
    public string Name { get; set; } = string.Empty!;

    public string User { get; set; } = string.Empty!;
}
