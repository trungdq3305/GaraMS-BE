using GaraMS.Data.ViewModels.AutheticateModel;
using GaraMS.Data.ViewModels.CreateReqModel;
using GaraMS.Service.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GaraMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [Route("create-user")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel userModel)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.CreateUser(token, userModel);
            return StatusCode(res.Code, res);
        }

        [HttpGet("logged-in-user")]
        public async Task<IActionResult> GetLoggedInUser()
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.GetLoggedInUser(token);
            return StatusCode(res.Code, res);
        }

        [HttpPost("confirm-status")]
        public async Task<IActionResult> ConfirmUserStatus(int userId)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.ConfirmUserStatus(token, userId);
            return StatusCode(res.Code, res);
        }
        [HttpPut]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var res = await _userService.ChangePassword(token, model);
            return StatusCode(res.Code, res);
        }
        [HttpPut("edit-user")]
        public async Task<IActionResult> EditUser([FromBody] EditUserModel model)
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var res = await _userService.EditUser(token, model);
            return StatusCode(res.Code, res);
        }
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetUserById(int id)
        {
            
            var res = await _userService.GetUserById(id);
            return Ok(res);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {

            var res = await _userService.GetAllAsync();
            return Ok(res);
        }
        [HttpPut("get-all")]
        public async Task<IActionResult> GetAll(int id, EditUserModel model)
        {

            var res = await _userService.EditUserById(id,model);
            return Ok(res);
        }

    }
}
