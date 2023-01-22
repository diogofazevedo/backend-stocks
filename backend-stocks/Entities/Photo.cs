namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Photo
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    [Required]
    public byte[] Bytes { get; set; } = Array.Empty<byte>();

    [StringLength(200)]
    public string Description { get; set; } = string.Empty!;

    [Required]
    [StringLength(20)]
    public string FileExtension { get; set; } = string.Empty!;
    public decimal Size { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty!;
}
