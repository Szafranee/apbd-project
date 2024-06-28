using Microsoft.AspNetCore.Mvc;
using Project_01.Domain.Clients;
using Project_01.Exceptions;
using Project_01.Interfaces;
using Project_01.RequestModels.Clients;

namespace Project_01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientService clientService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] CreateClientRequest createClientRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            Client client;
            if (createClientRequest.ClientType == "Individual")
            {
                if (string.IsNullOrEmpty(createClientRequest.LastName) || string.IsNullOrEmpty(createClientRequest.PESEL))
                {
                    return BadRequest("LastName and PESEL are required for Individual clients.");
                }
                client = new IndividualClient
                {
                    Name = createClientRequest.Name,
                    LastName = createClientRequest.LastName,
                    PESEL = createClientRequest.PESEL,
                    Address = createClientRequest.Address,
                    Email = createClientRequest.Email,
                    PhoneNumber = createClientRequest.PhoneNumber,
                    ClientType = ClientType.Individual
                };
            }
            else if (createClientRequest.ClientType == "Corporate")
            {
                if (string.IsNullOrEmpty(createClientRequest.KRS))
                {
                    return BadRequest("KRS is required for Corporate clients.");
                }
                client = new CorporateClient
                {
                    Name = createClientRequest.Name,
                    KRS = createClientRequest.KRS,
                    Address = createClientRequest.Address,
                    Email = createClientRequest.Email,
                    PhoneNumber = createClientRequest.PhoneNumber,
                    ClientType = ClientType.Corporate
                };
            }
            else
            {
                return BadRequest("Invalid client type. Must be 'Individual' or 'Corporate'.");
            }

            var addedClient = await clientService.AddClientAsync(client);
            return CreatedAtAction(nameof(GetClient), new { id = addedClient.Id }, addedClient);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientRequest updateClientRequest)
    {
        var existingClient = await clientService.GetClientByIdAsync(id);
        if (existingClient == null)
        {
            return NotFound();
        }

        existingClient.Name = updateClientRequest.Name;
        existingClient.Address = updateClientRequest.Address;
        existingClient.Email = updateClientRequest.Email;
        existingClient.PhoneNumber = updateClientRequest.PhoneNumber;

        if (existingClient is IndividualClient individualClient)
        {
            if (string.IsNullOrEmpty(updateClientRequest.LastName))
            {
                return BadRequest("LastName is required for Individual clients.");
            }
            if (individualClient.PESEL != updateClientRequest.PESEL)
            {
                return BadRequest("Cannot change PESEL number for Individual clients.");
            }
        }
        else if (existingClient is CorporateClient corporateClient)
        {
            if (corporateClient.KRS != updateClientRequest.KRS)
            {
                return BadRequest("Cannot change KRS number for Corporate clients.");
            }
        }


        var updatedClient = await clientService.UpdateClientAsync(existingClient);
        return Ok(updatedClient);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            await clientService.DeleteClientAsync(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetClient(int id)
    {
        var client = await clientService.GetClientByIdAsync(id);
        if (client == null)
        {
            return NotFound();
        }
        return Ok(client);
    }
}