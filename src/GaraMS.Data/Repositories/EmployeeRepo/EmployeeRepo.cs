using GaraMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.EmployeeRepo
{
    public class EmployeeRepo : IEmployeeRepo
    {
        private readonly GaraManagementSystemContext _context;

        public EmployeeRepo(GaraManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Specialized)
                .Include(e => e.ServiceEmployees)
                    .ThenInclude(se => se.Service)
                .ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Specialized)
                .Include(e => e.ServiceEmployees)
                    .ThenInclude(se => se.Service)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<List<Employee>> GetEmployeesBySpecializationAsync(int specializationId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Specialized)
                .Include(e => e.ServiceEmployees)
                    .ThenInclude(se => se.Service)
                .Where(e => e.SpecializedId == specializationId)
                .ToListAsync();
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        { 
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> AssignServiceToEmployeeAsync(int employeeId, int serviceId)
        {
            var serviceEmployee = await _context.ServiceEmployees
                .FirstOrDefaultAsync(se => se.EmployeeId == employeeId && se.ServiceId == serviceId);

            if (serviceEmployee != null)
                return true; // Already assigned

            serviceEmployee = new ServiceEmployee
            {
                EmployeeId = employeeId,
                ServiceId = serviceId
            };

            _context.ServiceEmployees.Add(serviceEmployee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveServiceFromEmployeeAsync(int employeeId, int serviceId)
        {
            var serviceEmployee = await _context.ServiceEmployees
                .FirstOrDefaultAsync(se => se.EmployeeId == employeeId && se.ServiceId == serviceId);

            if (serviceEmployee == null)
                return false;

            _context.ServiceEmployees.Remove(serviceEmployee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Service>> GetEmployeeServicesAsync(int employeeId)
        {
            var serviceEmployees = await _context.ServiceEmployees
                .Include(se => se.Service)
                .Where(se => se.EmployeeId == employeeId)
                .ToListAsync();

            return serviceEmployees.Select(se => se.Service).ToList();
        }
    }
}
