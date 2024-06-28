using Microsoft.AspNetCore.Mvc;
using Project_01.Domain.Contracts;
using Project_01.Exceptions;
using Project_01.Interfaces;
using Project_01.RequestModels.Contracts;

namespace Project_01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController(IContractService contractService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest createContractRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contract = new Contract
        {
            ClientId = createContractRequest.ClientId,
            ProductId = createContractRequest.ProductId,
            StartDate = createContractRequest.StartDate,
            EndDate = createContractRequest.EndDate,
            IsSigned = createContractRequest.IsSigned,
            SupportYears = createContractRequest.SupportYears
        };

        var createdContract = await contractService.CreateContractAsync(contract);

        return CreatedAtAction(nameof(GetContractById), new { id = createdContract.Id }, createdContract);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetContractById(int id)
    {
        var contract = await contractService.GetContractByIdAsync(id);
        if (contract is null)
        {
            return NotFound();
        }

        return Ok(contract);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContracts()
    {
        var contracts = await contractService.GetAllContractsAsync();
        return Ok(contracts);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateContract(int id, [FromBody] UpdateContractRequest updateContractRequest)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contract = new Contract
        {
            Id = id,
            ClientId = updateContractRequest.ClientId,
            ProductId = updateContractRequest.ProductId,
            StartDate = updateContractRequest.StartDate,
            EndDate = updateContractRequest.EndDate,
            TotalPrice = updateContractRequest.TotalPrice,
            IsSigned = updateContractRequest.IsSigned,
            SupportYears = updateContractRequest.SupportYears
        };

        try
        {
            await contractService.UpdateContractAsync(contract);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        try
        {
            await contractService.DeleteContractAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("calculate-price")]
    public async Task<IActionResult> CalculateContractPrice([FromQuery] int productId, [FromQuery] int supportYears, [FromQuery] int clientId)
    {
        var price = await contractService.CalculateContractPrice(productId, supportYears, clientId);
        return Ok(price);
    }

    private async Task<bool> ContractExists(int id)
    {
        var contract = await contractService.GetContractByIdAsync(id);
        return contract is not null;
    }
}