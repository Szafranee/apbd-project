using Microsoft.AspNetCore.Mvc;
using Project_01.Domain.Discounts;
using Project_01.Exceptions;
using Project_01.Interfaces;

namespace Project_01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController(IDiscountService discountService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddDiscount([FromBody] Discount discount)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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
    public async Task<IActionResult> UpdateDiscount(int id, [FromBody] Discount discount)
    {
        if (id != discount.Id)
        {
            return BadRequest("Id in the body does not match the id in the URL");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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