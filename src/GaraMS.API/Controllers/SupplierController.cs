using GaraMS.Data.ViewModels.ServiceModel;
using GaraMS.Data.ViewModels.SupplierModel;
using GaraMS.Service.Services.SupplierService;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SupplierController : ControllerBase 
	{
		private readonly ISupplierService _supplierService;

		public SupplierController(ISupplierService supplierService) 
		{
			_supplierService = supplierService;
		}

		[HttpGet("suppliers")]
		public async Task<IActionResult> GetAllSupplierServices()
		{
			var result = await _supplierService.GetAllSupplierAsync();
			return Ok(result);
		}

		[HttpGet("supplier/{id}")]
		public async Task<IActionResult> GetSupplierById(int id)
		{
			var result = await _supplierService.GetSupplierByIdAsync(id);
			if (!result.IsSuccess) return NotFound(result);
			return Ok(result);
		}

		[HttpPost("supplier")]
		public async Task<IActionResult> CreateSupplier([FromHeader] string authorization, [FromBody] SupplierModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _supplierService.CreateSupplierAsync(token , model);

			if (!result.IsSuccess) return BadRequest(result);

			return CreatedAtAction(nameof(GetSupplierById), new { id = ((dynamic)result.Data)?.SupplierId }, result);
		}

		[HttpPut("supplier/{id}")]
		public async Task<IActionResult> UpdateSupplier([FromHeader] string authorization, int id, [FromBody] SupplierModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _supplierService.UpdateSupplierAsync(token, id, model);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}

		[HttpDelete("supplier/{id}")]
		public async Task<IActionResult> DeleteSupplier([FromHeader] string authorization, int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _supplierService.DeleteSupplierAsync(token, id);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}
	}
}
