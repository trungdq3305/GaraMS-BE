using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.Validate
{
    public interface IValidateService
    {
        Task<ResultModel> IsUserNameUnique(string username);
        Task<ResultModel> IsEmailUnique(string Email);
        Task<ResultModel> IsPhoneValid(string phone);

    }
}
