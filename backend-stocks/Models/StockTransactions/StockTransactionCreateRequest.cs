namespace WebApi.Models.StockTransactions;

using System.ComponentModel.DataAnnotations;

public class StockTransactionCreateRequest
{
    public int? StockId { get; set; }

    [Required]
    public string ProductCode { get; set; } = string.Empty!;
    public decimal Quantity { get; set; }

    [Required]
    public string UnityCode { get; set; } = string.Empty!;
    public string? Lot { get; set; }
    public string? SerialNumber { get; set; }
    public string? LocationCode { get; set; }
    public string? Observation { get; set; }
    public string User { get; set; } = string.Empty!;
}
