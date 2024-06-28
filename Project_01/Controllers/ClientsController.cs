using Microsoft.AspNetCore.Mvc;
using Project_01.Domain.Clients;
using Project_01.DTOs;
using Project_01.Exceptions;
using Project_01.Interfaces;

namespace Project_01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientService clientService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] ClientDto clientDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            Client client;
            if (clientDto.ClientType == "Individual")
            {
                if (string.IsNullOrEmpty(clientDto.LastName) || string.IsNullOrEmpty(clientDto.PESEL))
                {
                    return BadRequest("LastName and PESEL are required for Individual clients.");
                }
                client = new IndividualClient
                {
                    Name = clientDto.Name,
                    LastName = clientDto.LastName,
                    PESEL = clientDto.PESEL,
                    Address = clientDto.Address,
                    Email = clientDto.Email,
                    PhoneNumber = clientDto.PhoneNumber,
                    ClientType = ClientType.Individual
                };
            }
            else if (clientDto.ClientType == "Corporate")
            {
                if (string.IsNullOrEmpty(clientDto.KRS))
                {
                    return BadRequest("KRS is required for Corporate clients.");
                }
                client = new CorporateClient
                {
                    Name = clientDto.Name,
                    KRS = clientDto.KRS,
                    Address = clientDto.Address,
                    Email = clientDto.Email,
                    PhoneNumber = clientDto.PhoneNumber,
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
    public async Task<IActionResult> UpdateClient(int id, [FromBody] ClientDto clientDto)
    {
        var existingClient = await clientService.GetClientByIdAsync(id);
        if (existingClient == null)
        {
            return NotFound();
        }

        existingClient.Name = clientDto.Name;
        existingClient.Address = clientDto.Address;
        existingClient.Email = clientDto.Email;
        existingClient.PhoneNumber = clientDto.PhoneNumber;

        if (existingClient is IndividualClient individualClient)
        {
            if (string.IsNullOrEmpty(clientDto.LastName))
            {
                return BadRequest("LastName is required for Individual clients.");
            }
            if (individualClient.PESEL != clientDto.PESEL)
            {
                return BadRequest("Cannot change PESEL number for Individual clients.");
            }
        }
        else if (existingClient is CorporateClient corporateClient)
        {
            if (corporateClient.KRS != clientDto.KRS)
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