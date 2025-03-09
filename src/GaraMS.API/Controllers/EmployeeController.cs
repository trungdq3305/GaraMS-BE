using GaraMS.Data.ViewModels.EmployeeModel;
using GaraMS.Service.Services.EmployeeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
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
    }
}