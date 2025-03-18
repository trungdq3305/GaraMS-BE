
using GaraMS.Data.ViewModels.ReportModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.ReportService
{
    public interface IReportService
    {
        Task<ResultModel> GetAllReportsAsync(string? token);
        Task<ResultModel> GetReportByIdAsync(string? token, int id);
        Task<ResultModel> GetReportsByCustomerAsync(string? token);
        Task<ResultModel> GetReportsByLoginAsync(string? token); 
        Task<ResultModel> CreateReportAsync(string? token, CreateReportModel model);
        Task<ResultModel> UpdateReportAsync(string? token, int id, UpdateReportModel model);
        Task<ResultModel> DeleteReportAsync(string? token, int id);
    }
}
