namespace WebApi.Models.Unities;

using System.ComponentModel.DataAnnotations;

public class UnityCreateRequest
{
    [Required]
    public string Code { get; set; } = string.Empty!;

    [Required]
    public string Name { get; set; } = string.Empty!;
    public int Decimals { get; set; }
    public string User { get; set; } = string.Empty!;
}
