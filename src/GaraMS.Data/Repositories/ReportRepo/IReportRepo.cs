using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.ReportModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.ReportRepo
{
    public interface IReportRepo
    {
        Task<List<Report>> GetAllReportsAsync();
        Task<Report> GetReportByIdAsync(int id);
        Task<List<Report>> GetReportsByCustomerAsync(int customerId);
        Task<Report> CreateReportAsync(Report report);
        Task<Report> UpdateReportAsync(Report report);
        Task<bool> DeleteReportAsync(int id);
    }
}
