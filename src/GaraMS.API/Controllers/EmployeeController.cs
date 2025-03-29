using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.EmployeeModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.EmployeeService;
using GaraMS.Service.Services.TokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly GaraManagementSystemContext _context;
        private readonly ITokenService _token;
        private readonly IAccountService _accountService;
        public EmployeeController(IEmployeeService employeeService, GaraManagementSystemContext context, ITokenService token, IAccountService accountService)
        {
            _employeeService = employeeService;
            _context = context;
            _token = token;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.GetAllEmployeesAsync(token);
            return StatusCode(result.Code, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.GetEmployeeByIdAsync(token, id);
            return StatusCode(result.Code, result);
        }

        [HttpGet("specialization/{specializationId}")]
        public async Task<IActionResult> GetEmployeesBySpecialization(int specializationId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.GetEmployeesBySpecializationAsync(token, specializationId);
            return StatusCode(result.Code, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeModel model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.CreateEmployeeAsync(token, model);
            return StatusCode(result.Code, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeModel model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.UpdateEmployeeAsync(token, id, model);
            return StatusCode(result.Code, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.DeleteEmployeeAsync(token, id);
            return StatusCode(result.Code, result);
        }

        [HttpPost("{id}/services/{serviceId}")]
        public async Task<IActionResult> AssignService(int id, int serviceId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.AssignServiceToEmployeeAsync(token, id, serviceId);
            return StatusCode(result.Code, result);
        }

        [HttpDelete("{id}/services/{serviceId}")]
        public async Task<IActionResult> RemoveService(int id, int serviceId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.RemoveServiceFromEmployeeAsync(token, id, serviceId);
            return StatusCode(result.Code, result);
        }

        [HttpGet("{id}/services")]
        public async Task<IActionResult> GetEmployeeServices(int id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _employeeService.GetEmployeeServicesAsync(token, id);
            return StatusCode(result.Code, result);
        }

        [HttpGet("{id}/specialization/")]
        public async Task<IActionResult> GetSpecializations()
        {
            
            var result = await _employeeService.GetAllSpeAsync();
            return Ok(result);
        }

        //thêm ca làm cho nhân viên
        [HttpPost("Add-Shift-To-Employee")]
        public async Task<IActionResult> AddShiftToEmployee([FromQuery] int employeeId, [FromQuery] int shiftId, [FromQuery] int month)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 2, 3 });
            var useid = Convert.ToInt32(decodeModel.userid);
            if (!isValidRole)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }

            
            var a = await _context.EmployeeShifts.FirstOrDefaultAsync(x => x.ShiftId == shiftId && x.EmployeeId == employeeId && x.Month == month);
            if (a != null)
            {
                return BadRequest(new { status = "Error", message = "Shift duplicate" });
            }
            if(month < 1 || month > 12)
            {
                return BadRequest(new { status = "Error", message = "invalid month" });
            }
            var es = new EmployeeShift
            {
                EmployeeId = employeeId,
                ShiftId = shiftId,
                Month = month
            };
            _context.EmployeeShifts.Add(es);
            await _context.SaveChangesAsync();
            return Ok(es);
        }

        [HttpGet("shift")]
        public async Task<IActionResult> GetShifts()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 2, 3 });
            var useid = Convert.ToInt32(decodeModel.userid);
            var shifts = await _context.Shifts
                .ToListAsync();

            return StatusCode(200, shifts);
        }

    }
}