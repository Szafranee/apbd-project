using Microsoft.EntityFrameworkCore;
using Project_01.Data;
using Project_01.Domain.Discounts;
using Project_01.Exceptions;
using Project_01.Interfaces;

namespace Project_01.Services;

public class DiscountService(DatabaseContext context) : IDiscountService
{
    public async Task<Discount?> AddDiscountAsync(Discount? discount)
    {
        context.Discounts.Add(discount);
        await context.SaveChangesAsync();
        return discount;
    }

    public async Task<Discount?> GetDiscountByIdAsync(int id)
    {
        return await context.Discounts.FindAsync(id);
    }

    public async Task<IEnumerable<Discount?>> GetAllDiscountsAsync()
    {
        return await context.Discounts.ToListAsync();
    }

    public async Task<Discount> UpdateDiscountAsync(Discount discount)
    {
        context.Discounts.Update(discount);
        await context.SaveChangesAsync();
        return discount;
    }

    public async Task DeleteDiscountAsync(int id)
    {
        var discount = await context.Discounts.FindAsync(id);
        if (discount == null)
        {
            throw new NotFoundException($"Discount with id: {id} not found");
        }

        context.Discounts.Remove(discount);
        await context.SaveChangesAsync();
    }

    public async Task<List<Discount?>> GetActiveDiscountsAsync(DateTime date)
    {
        return await context.Discounts
            .Where(d => d != null && d.StartDate <= date && d.EndDate >= date)
            .ToListAsync();
    }
}