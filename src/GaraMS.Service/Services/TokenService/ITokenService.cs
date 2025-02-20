using GaraMS.Data.ViewModels.TokenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.TokenService
{
    public interface ITokenService
    {
        TokenModel decode(string token);
    }
}
