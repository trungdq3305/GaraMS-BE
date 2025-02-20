using GaraMS.Data.ViewModels;
using GaraMS.Data.ViewModels.AppointmentDTO;
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
		public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDTO dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var result = await _appointmentService.CreateAppointmentAsync(dto);
			return CreatedAtAction(nameof(GetAppointmentById), new { id = result.AppointmentId }, result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var success = await _appointmentService.UpdateAppointmentAsync(id, dto);
			if (!success) return NotFound();

			return NoContent();
		}
        [HttpPut("Status-Update/{id}")]
        public async Task<IActionResult> UpdateAppointmentStatus(
    int id,
    [FromQuery] string status,
    [FromQuery] string reason)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _appointmentService.UpdateAppointmentStatusAsync(id, status, reason);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAppointment(int id)
		{
			var success = await _appointmentService.DeleteAppointmentAsync(id);
			if (!success) return NotFound();

			return NoContent();
		}
	}
}
