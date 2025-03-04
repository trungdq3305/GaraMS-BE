using GaraMS.Data.ViewModels;
using GaraMS.Data.ViewModels.AppointmentModel;
using GaraMS.Service;
using GaraMS.Service.Services.AppointmentService;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AppointmentsController : ControllerBase
	{
		private readonly IAppointmentService _appointmentService;

		public AppointmentsController(IAppointmentService appointmentService)
		{
			_appointmentService = appointmentService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAppointments()
		{
			var result = await _appointmentService.GetAllAppointmentsAsync();
			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAppointmentById(int id)
		{
			var result = await _appointmentService.GetAppointmentByIdAsync(id);
			if (result == null) return NotFound();
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateAppointment([FromBody] AppointmentModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _appointmentService.CreateAppointmentAsync(token, model);

			if (!result.IsSuccess) return BadRequest(result);

			return CreatedAtAction(nameof(GetAppointmentById), new { id = ((dynamic)result.Data)?.AppointmentId }, result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentModel model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _appointmentService.UpdateAppointmentAsync(token, id, model);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}
        [HttpPut("Status-Update/{id}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromQuery] string status, [FromQuery] string reason)
        {
			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _appointmentService.UpdateAppointmentStatusAsync(token, id, status, reason);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}

        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAppointment(int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault();
			var result = await _appointmentService.DeleteAppointmentAsync(token, id);

			if (!result.IsSuccess) return NotFound(result);

			return Ok(result);
		}
	}
}
