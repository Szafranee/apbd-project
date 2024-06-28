using Microsoft.EntityFrameworkCore;
using Project_01.Data;
using Project_01.Domain.Contracts;
using Project_01.Domain.Payments;
using Project_01.Exceptions;
using Project_01.Interfaces;

namespace Project_01.Services;

public class ContractService(DatabaseContext context, IDiscountService discountService) : IContractService
{
    public async Task<Contract> CreateContractAsync(Contract contract)
    {
        contract.TotalPrice =
            await CalculateContractPrice(contract.ProductId, contract.SupportYears, contract.ClientId);
        await context.Contracts.AddAsync(contract);
        await context.SaveChangesAsync();
        return contract;
    }

    public async Task<Contract?> GetContractByIdAsync(int id)
    {
        return await context.Contracts
            .Include(c => c.Client)
            .Include(c => c.Product)
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contract?>> GetAllContractsAsync()
    {
        return await context.Contracts
            .Include(c => c.Client)
            .Include(c => c.Product)
            .ToListAsync();
    }

    public async Task<Contract> UpdateContractAsync(Contract contract)
    {
        context.Contracts.Update(contract);
        await context.SaveChangesAsync();
        return contract;
    }

    public async Task DeleteContractAsync(int id)
    {
        var contract = await context.Contracts.FindAsync(id);
        if (contract == null)
        {
            throw new NotFoundException($"Contract with id: {id} not found");
        }

        context.Contracts.Remove(contract);
        await context.SaveChangesAsync();
    }

    public async Task<decimal> CalculateContractPrice(int productId, int supportYears, int clientId)
    {
        var product = await context.Products.FindAsync(productId);
        if (product == null)
        {
            throw new NotFoundException($"Product with id: {productId} not found");
        }

        var basePrice = product.YearlyLicenseCost;
        var totalPrice = basePrice + (supportYears * 1000);

        var activeDiscounts = await discountService.GetActiveDiscountsAsync(DateTime.Now);
        var maxDiscount = activeDiscounts.Max(d => d.PercentageValue);

        if (maxDiscount > 0)
        {
            totalPrice -= totalPrice * maxDiscount / 100;
        }

        // Apply returning customer discount
        var client = await context.Clients.FindAsync(clientId);
        if (client != null && await IsReturningCustomer(clientId))
        {
            totalPrice -= totalPrice * 0.05m;
        }

        return totalPrice;
    }

    public async Task<Contract?> GetActiveContractForClientAndProductAsync(int clientId, int productId)
    {
        return await context.Contracts
            .FirstOrDefaultAsync(c => c.ClientId == clientId && c.ProductId == productId && c.Status == ContractStatus.Active);
    }

    public async Task AddPaymentToContract(int id, Payment payment)
    {
        var contract = await context.Contracts
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contract == null)
        {
            throw new NotFoundException($"Contract with id: {id} not found");
        }

        contract.Payments.Add(payment);
        await context.SaveChangesAsync();
    }

    public async Task<bool> IsContractFullyPaid(int id)
    {
        var contract = await context.Contracts
            .Include(c => c.Payments)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contract == null)
        {
            throw new NotFoundException($"Contract with id: {id} not found");
        }

        return contract.Payments.Sum(p => p.Amount) == contract.TotalPrice;
    }

    private async Task<bool> IsReturningCustomer(int clientId)
    {
        return await context.Contracts
            .AnyAsync(c => c.ClientId == clientId && c.Status == ContractStatus.Completed);
    }
}