using GaraMS.Data.ViewModels.WarrantyHistoryModel;
using GaraMS.Service.Services.WarrantyHistoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class WarrantyHistoryController : ControllerBase
    {
		private readonly IWarrantyHistoryService _warrantyHistoryService;

		public WarrantyHistoryController(IWarrantyHistoryService warrantyHistoryService)
		{
			_warrantyHistoryService = warrantyHistoryService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllWarrantyHistories()
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _warrantyHistoryService.GetAllWarrantyHistoriesAsync(token);
			return StatusCode(result.Code, result);
		}

        [HttpGet("Warranty-of-login")]
        public async Task<IActionResult> GetWarrantyHistoriesOfLoginCustomer()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _warrantyHistoryService.GetByLoginAsync(token);
            return StatusCode(result.Code, result);
        }

        [HttpGet("{id}")]
		public async Task<IActionResult> GetWarrantyHistoryById(int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _warrantyHistoryService.GetWarrantyHistoryByIdAsync(token, id);
			return StatusCode(result.Code, result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateWarrantyHistory([FromBody] WarrantyHistoryModel model)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _warrantyHistoryService.CreateWarrantyHistoryAsync(token, model);
			return StatusCode(result.Code, result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateWarrantyHistory(int id, [FromBody] WarrantyHistoryModel model)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _warrantyHistoryService.UpdateWarrantyHistoryAsync(token, id, model);
			return StatusCode(result.Code, result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteWarrantyHistory(int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _warrantyHistoryService.DeleteWarrantyHistoryAsync(token, id);
			return StatusCode(result.Code, result);
		}

		[HttpPost("appointment/{appointmentId}")]
		public async Task<IActionResult> CreateWarrantyPeriodForAppointment(int appointmentId)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _warrantyHistoryService.CreateWarrantyPeriodForAppointmentAsync(token, appointmentId);
			return StatusCode(result.Code, result);
		}
	}
}
