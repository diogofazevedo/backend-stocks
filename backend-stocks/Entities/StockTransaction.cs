namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class StockTransaction
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

    [StringLength(200)]
    public string? Observation { get; set; }

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
