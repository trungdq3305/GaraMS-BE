using GaraMS.Data.ViewModels.PromotionModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels.VehicleModel;
using GaraMS.Service.Services.PromotionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("promotions")]
        public async Task<ActionResult> GetAllPromotions()
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _promotionService.GetAllPromotionsAsync(token);
            return StatusCode(res.Code, res);
        }

        [HttpGet("Active-promotions")]
        public async Task<ActionResult> GetActivePromotions()
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _promotionService.GetActivePromotionsAsync(token);
            return StatusCode(res.Code, res);
        }

        [HttpGet("promotion/{id}")]
        public async Task<ActionResult> GetPromotionById(int id)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _promotionService.GetPromotionByIdAsync(token, id);
            return StatusCode(res.Code, res);
        }

        [HttpPost("promotion")]
        public async Task<ActionResult> CreatePromotion([FromBody] PromotionModel promotionModel)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _promotionService.CreatePromotionAsync(token, promotionModel);
            return StatusCode(res.Code, res);
        }

        [HttpPut("promotion/{id}")]
        public async Task<ActionResult> UpdatePromotion(int id, [FromBody] UpdatePromotionModel promotionModel)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _promotionService.UpdatePromotionAsync(token, id, promotionModel);
            return StatusCode(res.Code, res);
        }

        [HttpDelete("promotion/{id}")]
        public async Task<ActionResult> DeletePromotion(int id)
        {
            string? token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _promotionService.DeletePromotionAsync(token, id);
            return StatusCode(res.Code, res);
        }
        [HttpGet("calculate-discount")]
        public async Task<IActionResult> CalculateDiscount([FromQuery] int serviceId, [FromQuery] decimal originalPrice)
        {
            try
            {
                string? token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var result = await _promotionService.CalculateServiceDiscountAsync(token, serviceId, originalPrice);

                if (!result.IsSuccess)
                {
                    return StatusCode(result.Code, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"Internal server error: {ex.Message}"
                });
            }

        }
    }
}
