using GaraMS.Data.ViewModels.AutheticateModel;
using GaraMS.Data.ViewModels.CreateReqModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.UserService
{
    public interface IUserService
    {
        Task<ResultModel> GetLoggedInUser(string token);
        Task<ResultModel> CreateUser(string token, CreateUserModel model);
        Task<ResultModel> ConfirmUserStatus(string token, int userId);
        Task<ResultModel> GetFalseUser(string token);
        Task<ResultModel> ChangePassword(string token, ChangePasswordModel model);
        Task<ResultModel> EditUser(string token, EditUserModel model);
        Task<ResultModel> GetUserById(int id);
        Task<ResultModel> EditUserById(int id, EditUserModel model);
        Task<ResultModel> GetAllAsync();
        Task<ResultModel> ConfirmWithCode(string email, string code);
        Task<ResultModel> RequestPasswordReset(string email);
        Task<ResultModel> VerifyResetCode(string email, string code);
        Task<ResultModel> ResetPassword(string email, string code, string newPassword);
        Task<ResultModel> RequestChangePassword(string token);
    }
}
