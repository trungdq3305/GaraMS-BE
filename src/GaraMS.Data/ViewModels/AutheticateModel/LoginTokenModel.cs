using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.AutheticateModel
{
    public class LoginTokenModel
    {
        public LoginResModel LoginResModel { get; set; }
        public string? token { get; set; }
    }
}
