using GaraMS.Data.Models;
using GaraMS.Data.Repositories.ReportRepo;
using GaraMS.Data.ViewModels.ReportModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.TokenService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.ReportService
{
    public class ReportService : IReportService
    {
        private readonly IReportRepo _reportRepo;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        public ReportService(IReportRepo reportRepo, IAccountService accountService, ITokenService tokenService)
        {
            _reportRepo = reportRepo;
            _accountService = accountService;
            _tokenService = tokenService;
        }

        private static ReportViewModel MapToViewModel(Report report)
        {
            return new ReportViewModel
            {
                ReportId = report.ReportId,
                Problem = report.Problem,
                Title = report.Title,
                Description = report.Description,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt,
                CustomerId = report.CustomerId,
                CustomerName = report.Customer?.User?.FullName
            };
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

        public async Task<ResultModel> GetAllReportsAsync(string token)
        {
            // Allow roles 3 (manager) to view all reports
            var validationResult = await ValidateToken(token, new List<int> { 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var reports = await _reportRepo.GetAllReportsAsync();
                var reportModels = reports.Select(MapToViewModel).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = reportModels,
                    Message = "Reports retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving reports: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> GetReportByIdAsync(string? token, int id)
        {
            var validationResult = await ValidateToken(token, new List<int> { 1, 2, 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var report = await _reportRepo.GetReportByIdAsync(id);
                if (report == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = $"Report with ID {id} not found"
                    };
                }

                var decodeModel = _tokenService.decode(token);
                if (decodeModel.role == "3" && report.CustomerId != int.Parse(decodeModel.userid))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Forbidden,
                        Message = "You don't have permission to view this report"
                    };
                }

                // Create a view model with the report data
                var reportViewModel = new ReportViewModel
                {
                    ReportId = report.ReportId,
                    Problem = report.Problem,
                    Title = report.Title,
                    Description = report.Description,
                    CreatedAt = report.CreatedAt,
                    UpdatedAt = report.UpdatedAt,
                    CustomerId = report.CustomerId,
                    CustomerName = report.Customer?.User?.FullName
                };

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = reportViewModel,  // Include the report data here
                    Message = "Report retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving report: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> GetReportsByCustomerAsync(string? token, int customerId)
        {
            var validationResult = await ValidateToken(token, new List<int> { 1, 2, 3 });
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            try
            {
                var decodeModel = _tokenService.decode(token);
                if (decodeModel.role == "3" && customerId != int.Parse(decodeModel.userid))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Forbidden,
                        Message = "You can only view your own reports"
                    };
                }

                var reports = await _reportRepo.GetReportsByCustomerAsync(customerId);
                var resultModels = reports.Select(MapToViewModel).ToList();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = resultModels,
                    Message = "Customer report retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving customer report: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> CreateReportAsync(string? token, CreateReportModel model)
        {
            var validationResult = await ValidateToken(token, new List<int> { 1, 2, 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var decodeModel = _tokenService.decode(token);
                if(decodeModel.role == "3" && model.CustomerId != int.Parse(decodeModel.userid))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Forbidden,
                        Message = "You can only create reports for yourself"
                    };
                }

                var report = new Report
                {
                    Problem = model.Problem,
                    Title = model.Title,
                    Description = model.Description,
                    CustomerId = model.CustomerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var createReport = await _reportRepo.CreateReportAsync(report);
                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.Created,
                    Data = MapToViewModel(createReport),
                    Message = "Report created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error creating report: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> UpdateReportAsync(string? token, int id, UpdateReportModel model)
        {
            var validationResult = await ValidateToken(token, new List<int> { 1, 2, 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var existingReport = await _reportRepo.GetReportByIdAsync(id);
                if (existingReport == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = $"Report with ID {id} not found"
                    };
                }

                // If customer role, verify they own the report
                var decodeModel = _tokenService.decode(token);
                if (decodeModel.role == "3" && existingReport.CustomerId != int.Parse(decodeModel.userid))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Forbidden,
                        Message = "You can only update your own reports"
                    };
                }

                existingReport.Problem = model.Problem;
                existingReport.Title = model.Title;
                existingReport.Description = model.Description;
                existingReport.UpdatedAt = DateTime.UtcNow;

                var updatedReport = await _reportRepo.UpdateReportAsync(existingReport);
                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Data = MapToViewModel(updatedReport),
                    Message = "Report updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error updating report: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> DeleteReportAsync(string? token, int id)
        {
            var validationResult = await ValidateToken(token, new List<int> { 1, 2, 3 });
            if (!validationResult.IsSuccess)
                return validationResult;

            try
            {
                var existingReport = await _reportRepo.GetReportByIdAsync(id);
                if (existingReport == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.NotFound,
                        Message = $"Report with ID {id} not found"
                    };
                }

                // If customer role, verify they own the report
                var decodeModel = _tokenService.decode(token);
                if (decodeModel.role == "3" && existingReport.CustomerId != int.Parse(decodeModel.userid))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Forbidden,
                        Message = "You can only delete your own reports"
                    };
                }

                await _reportRepo.DeleteReportAsync(id);
                return new ResultModel
                {
                    IsSuccess = true,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Report deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error deleting report: {ex.Message}"
                };
            }
        }
    }
}
