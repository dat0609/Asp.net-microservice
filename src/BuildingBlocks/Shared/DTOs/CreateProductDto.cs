using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class CreateProductDto: CreateOrUpdateProductDto
{
    [Required]
    public string No { get; set; }
}