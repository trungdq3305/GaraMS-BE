using GaraMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Skincare.Repositories.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.UserRepo
{
    public class UserRepo : GenericRepository<User>, IUserRepo
    {
        private readonly GaraManagementSystemContext _context;

        public UserRepo(GaraManagementSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == username);
        }
    } 

}
