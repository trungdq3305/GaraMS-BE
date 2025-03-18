using GaraMS.Data.Models;
using GaraMS.Data.ViewModels;
using GaraMS.Data.ViewModels.AppointmentModel;
using GaraMS.Service;
using GaraMS.Service.Services.AppointmentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
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
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _appointmentService.GetAllAppointmentsAsync(token);
			return StatusCode(result.Code, result);
		}
        [HttpGet("hahaha")]
        public async Task<IActionResult> GetAllAppoddintments()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _appointmentService.GetAllAppointmentsAsync(token);
            return StatusCode(result.Code, result);
        }
        [HttpGet("{id}")]
		public async Task<IActionResult> GetAppointmentById(int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _appointmentService.GetAppointmentByIdAsync(token, id);
			return StatusCode(result.Code, result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateAppointment([FromBody] AppointmentModel model)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _appointmentService.CreateAppointmentAsync(token, model);
			return StatusCode(result.Code, result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentModel model)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _appointmentService.UpdateAppointmentAsync(token, id, model);
			return StatusCode(result.Code, result);
		}
        [HttpPut("Status-Update/{id}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromQuery] string status, [FromQuery] string reason)
        {
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _appointmentService.UpdateAppointmentStatusAsync(token, id, status, reason);
			return StatusCode(result.Code, result);
		}

        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAppointment(int id)
		{
			var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			var result = await _appointmentService.DeleteAppointmentAsync(token, id);
			return StatusCode(result.Code, result);
		}
        [HttpGet("ViewVehiclebyLogin")]
        public async Task<ActionResult> ViewVehiclebyLogin()
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _appointmentService.GetAppointmentByLogin(token, new Appointment());
            return StatusCode(res.Code, res);
        }
    }
}
