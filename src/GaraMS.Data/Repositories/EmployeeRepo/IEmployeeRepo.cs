using GaraMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.EmployeeRepo
{
    public interface IEmployeeRepo
    {
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<List<Employee>> GetEmployeesBySpecializationAsync(int specializationId);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> AssignServiceToEmployeeAsync(int employeeId, int serviceId);
        Task<bool> RemoveServiceFromEmployeeAsync(int employeeId, int serviceId);
        Task<List<Service>> GetEmployeeServicesAsync(int employeeId);
    }
}
