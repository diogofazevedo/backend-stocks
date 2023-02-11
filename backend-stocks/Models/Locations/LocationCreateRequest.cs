﻿namespace WebApi.Models.Locations;

using System.ComponentModel.DataAnnotations;

public class LocationCreateRequest
{
    [Required]
    public string Code { get; set; } = string.Empty!;
    public string? Description { get; set; }
    public string User { get; set; } = string.Empty!;
}
