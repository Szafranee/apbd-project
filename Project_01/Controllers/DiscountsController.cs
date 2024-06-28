using Microsoft.AspNetCore.Mvc;
using Project_01.Domain.Discounts;
using Project_01.Exceptions;
using Project_01.Interfaces;
using Project_01.RequestModels.Discounts;

namespace Project_01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController(IDiscountService discountService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddDiscount([FromBody] CreateDiscountRequest createDiscountRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var discount = new Discount
        {
            Name = createDiscountRequest.Name,
            PercentageValue = createDiscountRequest.PercentageValue,
            StartDate = createDiscountRequest.StartDate,
            EndDate = createDiscountRequest.EndDate,
            ProductId = createDiscountRequest.ProductId
        };

        var addedDiscount = await discountService.AddDiscountAsync(discount);
        return CreatedAtAction(nameof(GetDiscount), new { id = addedDiscount.Id }, addedDiscount);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDiscount(int id)
    {
        var discount = await discountService.GetDiscountByIdAsync(id);
        if (discount == null)
        {
            return NotFound();
        }

        return Ok(discount);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDiscounts()
    {
        var discounts = await discountService.GetAllDiscountsAsync();
        return Ok(discounts);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDiscount(int id, [FromBody] UpdateDiscountRequest updateDiscountRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var discount = new Discount
        {
            Id = id,
            Name = updateDiscountRequest.Name,
            PercentageValue = updateDiscountRequest.PercentageValue,
            StartDate = updateDiscountRequest.StartDate,
            EndDate = updateDiscountRequest.EndDate,
            ProductId = updateDiscountRequest.ProductId
        };

        try
        {
            var updatedDiscount = await discountService.UpdateDiscountAsync(discount);
            return Ok(updatedDiscount);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        try
        {
            await discountService.DeleteDiscountAsync(id);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveDiscounts([FromQuery] DateTime date)
    {
        var discounts = await discountService.GetActiveDiscountsAsync(date);
        return Ok(discounts);
    }

    private async Task<bool> DiscountExists(int id)
    {
        var discount = await discountService.GetDiscountByIdAsync(id);
        return discount != null;
    }
}