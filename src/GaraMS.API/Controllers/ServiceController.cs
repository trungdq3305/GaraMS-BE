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
			return StatusCode(result.Code, result.Message);
		}

		[HttpGet("service/{id}")]
		public async Task<IActionResult> GetServiceById(int id)
		{
			var result = await _serviceService.GetServiceByIdAsync(id);
			return StatusCode(result.Code, result.Message);
		}

		[HttpPost("service")]
		public async Task<IActionResult> CreateService([FromHeader] string authorization, [FromBody] ServiceDTO serviceDto)
		{
			if (string.IsNullOrEmpty(authorization))
				return Unauthorized("Token is required");

			var result = await _serviceService.CreateServiceAsync(authorization, serviceDto);
			return StatusCode(result.Code, result.Message);
		}

		[HttpPut("service/{id}")]
		public async Task<IActionResult> UpdateService([FromHeader] string authorization, int id, [FromBody] ServiceDTO serviceDto)
		{
			if (string.IsNullOrEmpty(authorization))
				return Unauthorized("Token is required");

			var result = await _serviceService.UpdateServiceAsync(authorization, id, serviceDto);
			return StatusCode(result.Code, result.Message);
		}

		[HttpDelete("service/{id}")]
		public async Task<IActionResult> DeleteService([FromHeader] string authorization, int id)
		{
			if (string.IsNullOrEmpty(authorization))
				return Unauthorized("Token is required");

			var result = await _serviceService.DeleteServiceAsync(authorization, id);
			return StatusCode(result.Code, result.Message);
		}
	}
}
