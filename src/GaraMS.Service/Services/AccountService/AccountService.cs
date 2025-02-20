using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.ViewModels.AutheticateModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.HashPass;


namespace GaraMS.Service.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepo _userRepo;
        private readonly IAuthenticateService _authenticateService;
        public AccountService(IUserRepo userRepo, IAuthenticateService authenticateService)
        {
            _userRepo = userRepo;
            _authenticateService = authenticateService;
        }
        public async Task<ResultModel> LoginService(UserLoginReqModel user)
        {
            ResultModel res = new ResultModel();
            try
            {
                var existedUser = await _userRepo.GetByUsernameAsync(user.username);
                if (existedUser == null)
                {
                    res.IsSuccess = false;
                    res.Code = 400;
                    res.Message = "User not existed";
                    return res;
                }
                if (existedUser.Status.Equals(false))
                {
                    res.IsSuccess = false;
                    res.Code = 400;
                    res.Message = "You do not have access!";
                    return res;
                }
                bool isMatch = HashPass.HashPass.VerifyPassword(user.password, existedUser.Password);
                if (!isMatch)
                {
                    res.IsSuccess = false;
                    res.Code = 400;
                    res.Message = "Wrong password";
                    return res;
                }
                LoginResModel loginResModel = new LoginResModel()
                {
                    UserId = existedUser.UserId,
                    FullName = existedUser.FullName,
                    Role = existedUser.RoleId ?? 0,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    UpdatedAt = DateOnly.FromDateTime(DateTime.Now),
                    Email = existedUser.Email,
                    Phone = existedUser.PhoneNumber,
                    Address = existedUser.Address,
                    Status = existedUser.Status,
                };
                var token = _authenticateService.GenerateJWT(loginResModel);
                LoginTokenModel loginTokenModel = new LoginTokenModel()
                {
                    LoginResModel = loginResModel,
                    token = token
                };
                res.IsSuccess = true;
                res.Code = 200;
                res.Data = loginTokenModel;
                return res;
            }
            catch (Exception e)
            {
                res.IsSuccess = false;
                res.Code = 400;
                res.Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return res;
        }
        bool IAccountService.IsValidRole(string userRole, List<int> validRole)
        {
            return validRole.Any(role => role.ToString() == userRole);
        }
    }
}
