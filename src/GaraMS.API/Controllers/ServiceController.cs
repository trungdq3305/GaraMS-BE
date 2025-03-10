using GaraMS.Data.ViewModels;
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
			if (!result.IsSuccess) return NotFound(result);
			return Ok(result);
		}

		[HttpPost("service")]
		public async Task<IActionResult> CreateService([FromHeader] string authorization, [FromBody] ServiceModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _serviceService.CreateServiceAsync(token, model);

			if (!result.IsSuccess) return BadRequest(result);

			return CreatedAtAction(nameof(GetServiceById), new { id = ((dynamic)result.Data)?.ServiceId }, result);
		}

		[HttpPut("service/{id}")]
		public async Task<IActionResult> UpdateService([FromHeader] string authorization, int id, [FromBody] ServiceModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _serviceService.UpdateServiceAsync(token, id, model);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}

		[HttpDelete("service/{id}")]
		public async Task<IActionResult> DeleteService([FromHeader] string authorization, int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _serviceService.DeleteServiceAsync(token, id);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}
	}
}
