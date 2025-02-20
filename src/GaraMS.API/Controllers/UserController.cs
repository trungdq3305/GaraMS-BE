using GaraMS.Data.ViewModels.CreateReqModel;
using GaraMS.Service.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }

}
