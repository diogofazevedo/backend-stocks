namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;

public class Stock
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Product Product { get; set; } = new Product();
    public decimal Quantity { get; set; }

    [Required]
    public Unity Unity { get; set; } = new Unity();

    [StringLength(50)]
    public string? Lot { get; set; }

    [StringLength(50)]
    public string? SerialNumber { get; set; }
    public Location? Location { get; set; }
}
