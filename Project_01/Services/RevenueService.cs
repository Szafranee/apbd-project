using Microsoft.EntityFrameworkCore;
using Project_01.Data;
using Project_01.Domain.Contracts;
using Project_01.Domain.Payments;
using Project_01.Interfaces;

namespace Project_01.Services;

public class RevenueService(DatabaseContext databaseContext, IExchangeRateService exchangeRateService) : IRevenueService
{
    public async Task<decimal> CalculateCurrentRevenueAsync(int? productId = null, string currency = "PLN")
    {
        IQueryable<Contract> query = databaseContext.Contracts
            .Where(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.Completed)
            .Include(c => c.Payments);

        if (productId.HasValue)
        {
            query = query.Where(c => c.ProductId == productId.Value);
        }

        var contracts = await query.ToListAsync();

        var totalRevenue =
            contracts.Sum(c => c.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount));

        if (currency != "PLN")
        {
            var exchangeRate = await exchangeRateService.GetExchangeRateAsync(currency, "PLN");
            totalRevenue *= exchangeRate;
        }

        return totalRevenue;
    }

    public async Task<decimal> CalculatePredictedRevenueAsync(int? productId = null, string currency = "PLN")
    {
        var query = databaseContext.Contracts.AsQueryable();

        if (productId.HasValue)
        {
            query = query.Where(c => c.ProductId == productId.Value);
        }

        var contracts = await query.ToListAsync();

        var totalPredictedRevenue = contracts.Sum(c => c.TotalPrice);

        if (currency != "PLN")
        {
            var exchangeRate = await exchangeRateService.GetExchangeRateAsync(currency, "PLN");
            totalPredictedRevenue *= exchangeRate;
        }

        return totalPredictedRevenue;
    }
}