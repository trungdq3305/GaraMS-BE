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
        public Task<User> GetLoginAsync(int id);
        public Task<User?> GetByUsernameAsync(string username);
        public Task<List<User>> GetAllUser();
        public Task<List<Customer>> GetAllCustomer();
        public Task<List<Employee>> GetAllEmployee();
        public Task<List<Manager>> GetAllManager();
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Manager> AddManagerAsync(Manager manager);
    }
}
