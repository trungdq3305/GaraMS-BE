using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.AutheticateModel
{
    public class LoginResModel
    {
        public int UserId { get; set; }
        public int Role { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
        public DateOnly UpdatedAt { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool? Status { get; set; }
    }
}
