using Project_01.Domain.Discounts;

namespace Project_01.Domain.Products;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public string Category { get; set; }
    public decimal YearlyLicenseCost { get; set; }

    public ICollection<Discount> Discounts { get; set; }
}