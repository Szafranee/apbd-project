using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Project_01.RequestModels.Discounts;

public class UpdateDiscountRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    [Range(0, 100), Precision(5, 2)]
    public decimal PercentageValue { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public int? ProductId { get; set; }
}