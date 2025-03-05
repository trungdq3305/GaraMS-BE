using GaraMS.Data.ViewModels.InventoryModel;
using GaraMS.Data.ViewModels.ServiceModel;
using GaraMS.Service.Services.InventoryService;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InventoryController : ControllerBase
	{
		private readonly IInventoryService _inventoryService;

		public InventoryController(IInventoryService inventoryService)
		{
			_inventoryService = inventoryService;
		}

		[HttpGet("inventories")]
		public async Task<IActionResult> GetAllInventories()
		{
			var result = await _inventoryService.GetAllInventoriesAsync();
			return Ok(result);
		}

		[HttpGet("inventory/{id}")]
		public async Task<IActionResult> GetInventoryById(int id)
		{
			var result = await _inventoryService.GetInventoryByIdAsync(id);
			if (!result.IsSuccess) return NotFound(result);
			return Ok(result);
		}

		[HttpPost("inventory")]
		public async Task<IActionResult> CreateInventory([FromHeader] string authorization, [FromBody] InventoryModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _inventoryService.CreateInventoryAsync(token, model);

			if (!result.IsSuccess) return BadRequest(result);

			return CreatedAtAction(nameof(GetInventoryById), new { id = ((dynamic)result.Data)?.InventoryId }, result);
		}

		[HttpPut("inventory/{id}")]
		public async Task<IActionResult> UpdateInventory([FromHeader] string authorization, int id, [FromBody] InventoryModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _inventoryService.UpdateInventoryAsync(token, id, model);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}

		[HttpDelete("inventory/{id}")]
		public async Task<IActionResult> DeleteService([FromHeader] string authorization, int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _inventoryService.DeleteInventoryAsync(token, id);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}
	}
}
