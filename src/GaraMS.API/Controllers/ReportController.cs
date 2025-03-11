using GaraMS.Data.ViewModels.ReportModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.ReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _reportService.GetAllReportsAsync(token);
                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                });
            }
        }

        [HttpGet("report/{id}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            try
            {
                string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _reportService.GetReportByIdAsync(token, id);
                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReportsByCustomer(int customerId)
        {
            try
            {
                string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _reportService.GetReportsByCustomerAsync(token, customerId);
                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPost("report")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _reportService.CreateReportAsync(token, model);
                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                });
            }
        }

        [HttpPut("report/{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] UpdateReportModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _reportService.UpdateReportAsync(token, id, model);
                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                });
            }
        }

        [HttpDelete("report/{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _reportService.DeleteReportAsync(token, id);
                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "Internal server error"
                });
            }
        }
    }
}
