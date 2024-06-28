using Project_01.Domain.Products;

namespace Project_01.Domain.Discounts;

public class Discount
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal PercentageValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int? ProductId { get; set; }

    public Product? Product { get; set; }
}