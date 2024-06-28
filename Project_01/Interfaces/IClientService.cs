using Project_01.Domain.Clients;

namespace Project_01.Interfaces;

public interface IClientService
{
    Task<Client?> AddClientAsync(Client? client);
    Task<Client?> UpdateClientAsync(Client? client);
    Task DeleteClientAsync(int id);
    Task<Client?> GetClientByIdAsync(int id);
    Task<IEnumerable<Client>?> GetClientsAsync();
}