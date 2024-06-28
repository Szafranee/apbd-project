using Project_01.Domain.Products;

namespace Project_01.Interfaces;

public interface IProductService
{
    Task<Product?> GetProductByIdAsync(int id);
    Task<IEnumerable<Product>?> GetAllProductsAsync();
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<decimal> GetProductPriceAsync(int id);
}