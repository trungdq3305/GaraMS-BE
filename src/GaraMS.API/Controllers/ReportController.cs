using GaraMS.Data.ViewModels.ReportModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.ReportService;
using GaraMS.Service.Services.TokenService;
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
        private readonly ITokenService _tokenService;
        public ReportController(IReportService reportService, ITokenService tokenService)
        {
            _reportService = reportService;
            _tokenService = tokenService;
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
        [HttpGet("debug-token")]
        public IActionResult DebugToken()
        {
            try
            {
                string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var decodeModel = _tokenService.decode(token);
                return Ok(new
                {
                    UserId = decodeModel.userid,
                    Role = decodeModel.role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
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
                var result = await _reportService.GetReportsByCustomerAsync(token);
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

        [HttpGet("my-reports")]
        public async Task<IActionResult> GetReportsByLogin()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var result = await _reportService.GetReportsByLoginAsync(token);

            if (!result.IsSuccess)
                return StatusCode(result.Code, result);

            return StatusCode(result.Code, result);
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
