using GaraMS.Data.ViewModels.AutheticateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.AutheticateService
{
    public interface IAuthenticateService
    {
        string GenerateJWT(LoginResModel User);
        string decodeToken(string jwtToken, string nameClaim);
    }
}
