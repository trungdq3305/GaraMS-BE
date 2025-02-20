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
        public async Task<List<User>> GetAllUser()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserName == username);
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    } 

}
