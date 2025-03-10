using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.VehicleModel;
using GaraMS.Service.Services.VehicleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("ViewListVehicle")]
        public async Task<ActionResult> ViewListVehicle([FromQuery] VehicleSearch vehicleSearch)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _vehicleService.ViewListVehicle(token, vehicleSearch);
            return StatusCode(res.Code, res);
        }

        [HttpGet("ViewVehiclebyLogin")]
        public async Task<ActionResult> ViewVehiclebyLogin([FromQuery] Vehicle vehicle)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _vehicleService.ViewListVehicleByLogin(token, vehicle);
            return StatusCode(res.Code, res);
        }

        [HttpPost("CreateVehicle")]
        public async Task<ActionResult> CreateVehicle([FromBody] CreateVehicle vehicle)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _vehicleService.CreateVehicle(token, vehicle);
            return StatusCode(res.Code, res);
        }
        [HttpPut("EditVehicle")]
        public async Task<ActionResult> EditVehicle([FromBody] EditVehicle vehicle)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _vehicleService.EditVehicle(token, vehicle);
            return StatusCode(res.Code, res);
        }

    }
}
