using Project_01.Domain.Contracts;

namespace Project_01.Interfaces;

public interface IContractService
{
    Task<Contract> CreateContractAsync(Contract contract);
    Task<Contract?> GetContractByIdAsync(int id);
    Task<IEnumerable<Contract?>> GetAllContractsAsync();
    Task<Contract> UpdateContractAsync(Contract contract);
    Task DeleteContractAsync(int id);
    Task<decimal> CalculateContractPrice(int productId, int supportYears, int clientId);
}