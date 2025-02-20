using GaraMS.Data.ViewModels.ServiceDTO;
using GaraMS.Service.Services.ServiceService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GaraMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceController : ControllerBase
	{
		private readonly IServiceService _serviceService;

		public ServiceController(IServiceService serviceService)
		{
			_serviceService = serviceService;
		}

		[HttpGet("services")]
		public async Task<IActionResult> GetAllServices()
		{
			var result = await _serviceService.GetAllServicesAsync();
			return Ok(result);
		}

		[HttpGet("service/{id}")]
		public async Task<IActionResult> GetServiceById(int id)
		{
			var result = await _serviceService.GetServiceByIdAsync(id);
			if (result == null) return NotFound("Service not found");
			return Ok(result);
		}

		[HttpPost("service")]
		public async Task<IActionResult> CreateService([FromBody] ServiceDTO serviceDto)
		{
			var result = await _serviceService.CreateServiceAsync(serviceDto);
			if (!result) return BadRequest("Failed to create service");
			return Ok("Service created successfully");
		}

		[HttpPut("service/{id}")]
		public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDTO serviceDto)
		{
			var result = await _serviceService.UpdateServiceAsync(id, serviceDto);
			if (!result) return NotFound("Service not found or update failed");
			return Ok("Service updated successfully");
		}

		[HttpDelete("service/{id}")]
		public async Task<IActionResult> DeleteService(int id)
		{
			var result = await _serviceService.DeleteServiceAsync(id);
			if (!result) return NotFound("Service not found");
			return Ok("Service deleted successfully");
		}
	}
}
