using Project_01.Domain.Discounts;

namespace Project_01.Interfaces;

public interface IDiscountService
{
    Task<Discount?> AddDiscountAsync(Discount? discount);
    Task<Discount?> GetDiscountByIdAsync(int id);
    Task<IEnumerable<Discount?>> GetAllDiscountsAsync();
    Task<Discount> UpdateDiscountAsync(Discount discount);
    Task DeleteDiscountAsync(int id);
    Task<List<Discount?>> GetActiveDiscountsAsync(DateTime date);
}