using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_01.Interfaces;
using Project_01.Services;

namespace Project_01.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RevenueController(IRevenueService revenueService) : ControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentRevenue([FromQuery] int? productId, [FromQuery] string currency)
    {
        var revenue = await revenueService.CalculateCurrentRevenueAsync(productId, currency);
        return Ok(new { Revenue = revenue, Currency = currency });
    }

    [HttpGet("predicted")]
    public async Task<IActionResult> GetPredictedRevenue([FromQuery] int? productId, [FromQuery] string currency)
    {
        var revenue = await revenueService.CalculatePredictedRevenueAsync(productId, currency);
        return Ok(new { Revenue = revenue, Currency = currency });
    }
}