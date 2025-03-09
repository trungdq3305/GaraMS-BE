using GaraMS.Data.ViewModels.EmployeeModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Task<ResultModel> GetAllEmployeesAsync(string? token);
        Task<ResultModel> GetEmployeeByIdAsync(string? token, int id);
        Task<ResultModel> GetEmployeesBySpecializationAsync(string? token, int specializationId);
        Task<ResultModel> CreateEmployeeAsync(string? token, CreateEmployeeModel model);
        Task<ResultModel> UpdateEmployeeAsync(string? token, int id, UpdateEmployeeModel model);
        Task<ResultModel> DeleteEmployeeAsync(string? token, int id);
        Task<ResultModel> AssignServiceToEmployeeAsync(string? token, int employeeId, int serviceId);
        Task<ResultModel> RemoveServiceFromEmployeeAsync(string? token, int employeeId, int serviceId);
        Task<ResultModel> GetEmployeeServicesAsync(string? token, int employeeId);
    }
}
