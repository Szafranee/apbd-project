using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_01.Domain.Contracts;
using Project_01.Domain.Payments;
using Project_01.Exceptions;
using Project_01.Interfaces;
using Project_01.RequestModels;
using Project_01.RequestModels.Contracts;
using Project_01.RequestModels.Payments;

namespace Project_01.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContractsController(
    IContractService contractService,
    IClientService clientService,
    IProductService productService,
    IDiscountService discountService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest createContractRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if client exists
        var client = await clientService.GetClientByIdAsync(createContractRequest.ClientId);
        if (client is null)
        {
            return BadRequest("Client not found");
        }

        // Check if product exists
        var product = await productService.GetProductByIdAsync(createContractRequest.ProductId);
        if (product is null)
        {
            return BadRequest("Product not found");
        }

        // check if client has active contract on this product
        var activeContract =
            await contractService.GetActiveContractForClientAndProductAsync(createContractRequest.ClientId,
                createContractRequest.ProductId);
        if (activeContract is not null)
        {
            return BadRequest("Client already has an active contract on this product");
        }

        // check if support if extended support is valid (0 to 3 years - integers)
        if (createContractRequest.SupportYears is < 0 or > 3 || createContractRequest.SupportYears % 1 != 0)
        {
            return BadRequest("Support years must be between 0 and 3 years and must be an whole number");
        }

        // calculate total price
        var totalPrice = await contractService.CalculateContractPrice(createContractRequest.ProductId,
            createContractRequest.SupportYears, createContractRequest.ClientId);

        var contract = new Contract
        {
            ClientId = createContractRequest.ClientId,
            ProductId = createContractRequest.ProductId,
            StartDate = createContractRequest.StartDate,
            EndDate = createContractRequest.EndDate,
            TotalPrice = totalPrice,
            IsSigned = createContractRequest.IsSigned,
            SupportYears = createContractRequest.SupportYears,
            Status = ContractStatus.Pending
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

    /*[HttpPut("{id:int}")]
    public Task<IActionResult> UpdateContract(int id)
    {
        return Task.FromResult<IActionResult>(BadRequest("Contract cannot be updated"));
    }*/

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
    public async Task<IActionResult> CalculateContractPrice([FromQuery] int productId, [FromQuery] int supportYears,
        [FromQuery] int clientId)
    {
        var price = await contractService.CalculateContractPrice(productId, supportYears, clientId);
        return Ok(price);
    }

    [HttpPost("{id:int}/payments")]
    public async Task<IActionResult> AddPaymentToContractAsync(int id, [FromBody] AddPaymentRequest paymentRequest)
    {
        var contract = await contractService.GetContractByIdAsync(id);
        if (contract == null)
        {
            return NotFound();
        }

        if (contract.Status != ContractStatus.Pending)
        {
            return BadRequest("Can only add payments to pending contracts");
        }

        if (DateTime.Now > contract.EndDate)
        {
            return BadRequest("Contract payment period has expired");
        }

        if (contract.Payments.Sum(p => p.Amount) == contract.TotalPrice)
        {
            throw new BadRequestException("Contract is already paid in full");
        }

        if (contract.Payments.Sum(p => p.Amount) + paymentRequest.Amount > contract.TotalPrice)
        {
            throw new BadRequestException("Payment amount exceeds contract total price");
        }

        if (DateTime.Now < contract.StartDate || DateTime.Now > contract.EndDate)
        {
            // return all payments to the client
            contract.Payments.Clear();
            await contractService.UpdateContractAsync(contract);
            throw new BadRequestException("Payment date is not within contract period. All payments have been returned to the client. Please create a new contract.");
        }

        var payment = new Payment
        {
            ContractId = id,
            Amount = paymentRequest.Amount,
            PaymentDate = DateTime.Now,
            Status = PaymentStatus.Completed
        };

        await contractService.AddPaymentToContract(id, payment);

        // check if contract is fully paid
        if (await contractService.IsContractFullyPaid(id))
        {
            contract.Status = ContractStatus.Active;
            contract.IsSigned = true;
            await contractService.UpdateContractAsync(contract);
        }

        return Ok(payment);
    }

    private async Task<bool> ContractExists(int id)
    {
        var contract = await contractService.GetContractByIdAsync(id);
        return contract is not null;
    }
}