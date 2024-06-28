using Project_01.Data;
using Project_01.Domain.Clients;
using Project_01.Exceptions;
using Project_01.Interfaces;

namespace Project_01.Services;

public class ClientService(DatabaseContext context) : IClientService
{
    public async Task<Client?> AddClientAsync(Client? client)
    {
        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();
        return client;
    }

    public async Task<Client?> UpdateClientAsync(Client? client)
    {
        context.Clients.Update(client);
        await context.SaveChangesAsync();
        return client;
    }

    public async Task DeleteClientAsync(int id)
    {
        var client = await context.Clients.FindAsync(id);
        switch (client)
        {
            case null:
                throw new NotFoundException($"Client with id: {id} not found");
            case IndividualClient individualClient:
                individualClient.IsDeleted = true;
                individualClient.Name = "[DELETED]";
                individualClient.LastName = "[DELETED]";
                individualClient.Address = "[DELETED]";
                individualClient.Email = "[DELETED]";
                individualClient.PhoneNumber = "[DELETED]";
                individualClient.PESEL = "[DELETED]";

                context.Clients.Update(individualClient);
                break;
            case CorporateClient corporateClient:
                throw new InvalidOperationException("Corporate clients cannot be deleted");
        }

        await context.SaveChangesAsync();
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        return await context.Clients.FindAsync(id);
    }
}