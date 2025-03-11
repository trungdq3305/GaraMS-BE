using GaraMS.Data.ViewModels.DashboardModel;
using GaraMS.Service.Services.DashboardService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardModel>> GetDashboardData()
    {
        try
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var dashboard = await _dashboardService.GetDashboardDataAsync(token);
            return StatusCode(dashboard.Code, dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard data");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("top-services")]
    public async Task<ActionResult<List<TopServiceDTO>>> GetTopServices([FromQuery] int count = 5)
    {
        try
        {
            var topServices = await _dashboardService.GetTopServicesAsync(count);
            return Ok(topServices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top services");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<decimal>> GetRevenue(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var revenue = await _dashboardService.GetTotalRevenueAsync(startDate, endDate);
            return Ok(revenue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving revenue data");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("recent-appointments")]
    public async Task<ActionResult<List<RecentAppointmentDTO>>> GetRecentAppointments(
        [FromQuery] int count = 5)
    {
        try
        {
            var appointments = await _dashboardService.GetRecentAppointmentsAsync(count);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent appointments");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}