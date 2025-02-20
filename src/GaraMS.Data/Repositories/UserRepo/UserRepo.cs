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

        public async Task<User> GetLoginAsync(int id)
        {
            return await _context.Users.Include(u => u.Garas).FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task<List<User>> GetAllUser()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<List<Customer>> GetAllCustomer()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }
        public async Task<List<Employee>> GetAllEmployee()
        {
            return await _context.Employees.AsNoTracking().ToListAsync();
        }
        public async Task<List<Manager>> GetAllManager()
        {
            return await _context.Managers.AsNoTracking().ToListAsync();
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
        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
        public async Task<Manager> AddManagerAsync(Manager manager)
        {
            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();
            return manager;
        }
        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }
    } 

}
