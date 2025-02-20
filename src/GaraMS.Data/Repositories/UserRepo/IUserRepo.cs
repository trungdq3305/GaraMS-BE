using GaraMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.UserRepo
{
    public interface IUserRepo
    {
        public Task<User?> GetByUsernameAsync(string username);
        public Task<List<User>> GetAllUser();
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
    }
}
