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

    }
}
