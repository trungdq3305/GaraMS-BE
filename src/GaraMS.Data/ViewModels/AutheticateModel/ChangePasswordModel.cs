﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.AutheticateModel
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmationCode { get; set; }
    }
}
