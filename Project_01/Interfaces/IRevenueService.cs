namespace Project_01.Interfaces;

public interface IRevenueService
{
    Task<decimal> CalculateCurrentRevenueAsync(int? productId = null, string currency = "PLN");
    Task<decimal> CalculatePredictedRevenueAsync(int? productId = null, string currency = "PLN");
}