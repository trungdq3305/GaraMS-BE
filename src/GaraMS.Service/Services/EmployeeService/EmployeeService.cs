using GaraMS.Data.Models;
using GaraMS.Data.Repositories.EmployeeRepo;
using GaraMS.Data.ViewModels.EmployeeModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.TokenService;
using System.Net;

namespace GaraMS.Service.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public EmployeeService(
            IEmployeeRepo employeeRepo,
            IAccountService accountService,
            ITokenService tokenService)
        {
            _employeeRepo = employeeRepo;
            _accountService = accountService;
            _tokenService = tokenService;
        }

        private ResultModel UnauthorizedResult => new()
        {
            IsSuccess = false,
            Code = (int)HttpStatusCode.Unauthorized,
            Message = "Invalid token."
        };

        private async Task<ResultModel> ValidateToken(string? token, List<int> allowedRoles)
        {
            var decodeModel = _tokenService.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, allowedRoles);

            if (!isValidRole)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Forbidden,
                    Message = "You don't have permission to perform this action."
                };
            }

            if (!int.TryParse(decodeModel.userid, out int userId) || userId <= 0)
            {
                return UnauthorizedResult;
            }

            return new ResultModel { IsSuccess = true };
        }

        public async Task<ResultModel> GetAllEmployeesAsync(string? token)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var employees = await _employeeRepo.GetAllEmployeesAsync();
                var employeeModels = employees.Select(e => new EmployeeModel
                {
                    EmployeeId = e.EmployeeId,
                    Salary = e.Salary,
                    SpecializedId = e.SpecializedId,
                    SpecializedName = e.Specialized?.SpecializedName,
                    UserId = e.UserId,
                    UserName = e.User?.FullName,
                    UserEmail = e.User?.Email,
                    Services = e.ServiceEmployees?.Select(se => new ServiceModel
                    {
                        ServiceId = se.Service.ServiceId,
                        ServiceName = se.Service.ServiceName,
                        Description = se.Service.Description,
                        TotalPrice = se.Service.TotalPrice,
                    }).ToList() ?? new List<ServiceModel>()
                }).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = employees,
                    Message = "Get all employees successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error getting employees: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> GetEmployeeByIdAsync(string? token, int id)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var employee = await _employeeRepo.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = $"Employee with ID {id} not found"
                    };
                }

                var employeeModel = new EmployeeModel
                {
                    EmployeeId = employee.EmployeeId,
                    Salary = employee.Salary,
                    SpecializedId = employee.SpecializedId,
                    SpecializedName = employee.Specialized?.SpecializedName,
                    UserId = employee.UserId,
                    UserName = employee.User?.FullName,
                    UserEmail = employee.User?.Email,
                    Services = employee.ServiceEmployees?.Select(se => new ServiceModel
                    {
                        ServiceId = se.Service.ServiceId,
                        ServiceName = se.Service.ServiceName,
                        Description = se.Service.Description,
                        TotalPrice = se.Service.TotalPrice
                    }).ToList() ?? new List<ServiceModel>()
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = employeeModel,
                    Message = "Get employee successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error getting employee: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> GetEmployeesBySpecializationAsync(string? token, int specializationId)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var employees = await _employeeRepo.GetEmployeesBySpecializationAsync(specializationId);
                var employeeModels = employees.Select(e => new EmployeeModel
                {
                    EmployeeId = e.EmployeeId,
                    Salary = e.Salary,
                    SpecializedId = e.SpecializedId,
                    SpecializedName = e.Specialized?.SpecializedName,
                    UserId = e.UserId,
                    UserName = e.User?.FullName,
                    UserEmail = e.User?.Email,
                    Services = e.ServiceEmployees?.Select(se => new ServiceModel
                    {
                        ServiceId = se.Service.ServiceId,
                        ServiceName = se.Service.ServiceName,
                        Description = se.Service.Description,
                        TotalPrice = se.Service.TotalPrice
                    }).ToList() ?? new List<ServiceModel>()
                }).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = employeeModels,
                    Message = $"Get employees by specialization {specializationId} successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error getting employees by specialization: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> CreateEmployeeAsync(string? token, CreateEmployeeModel model)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var employee = new Employee
                {
                    Salary = model.Salary,
                    SpecializedId = model.SpecializedId,
                    UserId = model.UserId
                };

                var result = await _employeeRepo.CreateEmployeeAsync(employee);
                return await GetEmployeeByIdAsync(token, result.EmployeeId);
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error creating employee: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> UpdateEmployeeAsync(string? token, int id, UpdateEmployeeModel model)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var existingEmployee = await _employeeRepo.GetEmployeeByIdAsync(id);
                if (existingEmployee == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = $"Employee with ID {id} not found"
                    };
                }

                existingEmployee.Salary = model.Salary ?? existingEmployee.Salary;
                existingEmployee.SpecializedId = model.SpecializedId ?? existingEmployee.SpecializedId;

                var result = await _employeeRepo.UpdateEmployeeAsync(existingEmployee);
                return await GetEmployeeByIdAsync(token, result.EmployeeId);
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error updating employee: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> DeleteEmployeeAsync(string? token, int id)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var result = await _employeeRepo.DeleteEmployeeAsync(id);
                if (!result)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = $"Employee with ID {id} not found"
                    };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Delete employee successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error deleting employee: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> AssignServiceToEmployeeAsync(string? token, int employeeId, int serviceId)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var result = await _employeeRepo.AssignServiceToEmployeeAsync(employeeId, serviceId);
                if (!result)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Failed to assign service to employee"
                    };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Service assigned successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error assigning service: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> RemoveServiceFromEmployeeAsync(string? token, int employeeId, int serviceId)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var result = await _employeeRepo.RemoveServiceFromEmployeeAsync(employeeId, serviceId);
                if (!result)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = "Service assignment not found"
                    };
                }

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Service removed successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error removing service: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> GetEmployeeServicesAsync(string? token, int employeeId)
        {
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var services = await _employeeRepo.GetEmployeeServicesAsync(employeeId);
                var serviceModels = services.Select(s => new ServiceModel
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Description = s.Description,
                    TotalPrice = s.TotalPrice
                }).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = serviceModels,
                    Message = "Get employee services successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error getting employee services: {ex.Message}"
                };
            }
        }

        public async Task<List<Specialized>> GetAllSpeAsync()
        {
            return await _employeeRepo.GetAllSpecializationsAsync();
        }
    }
}