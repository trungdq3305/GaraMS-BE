using GaraMS.Data.ViewModels.AutheticateModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using Microsoft.AspNetCore.Mvc;

namespace GaraMS.API.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class AuthorizeController : Controller
    {
        private readonly IAccountService _accountService;
        public AuthorizeController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReqModel user)
        {
            ResultModel resultModel = await _accountService.LoginService(user);
            return resultModel.IsSuccess ? Ok(resultModel) : BadRequest(resultModel);
        }
    }
}
