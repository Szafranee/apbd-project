using Microsoft.EntityFrameworkCore;
using Project_01.Data;
using Project_01.Domain.Products;
using Project_01.Exceptions;
using Project_01.Interfaces;

namespace Project_01.Services;

public class ProductService(DatabaseContext context) : IProductService
{
    public async Task<Product> GetProductByIdAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }
        return product;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await context.Products.ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        context.Products.Update(product);
        try {
            await context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException) {
            throw new NotFoundException($"Product with id {product.Id} not found");
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }

    public async Task<decimal> GetProductPriceAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            throw new NotFoundException($"Product with id {id} not found");
        }
        return product.YearlyLicenseCost;
    }
}