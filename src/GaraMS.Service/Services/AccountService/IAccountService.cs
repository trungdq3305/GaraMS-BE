using GaraMS.Data.ViewModels.AutheticateModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.AccountService
{
    public interface IAccountService
    {
        public Task<ResultModel> LoginService(UserLoginReqModel user);
        public bool IsValidRole(string userRole, List<int> validRole);
    }
}
