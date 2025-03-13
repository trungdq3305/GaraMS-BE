using GaraMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.ReportRepo
{
    public class ReportRepo : IReportRepo
    {
        private readonly GaraManagementSystemContext _context;

        public ReportRepo(GaraManagementSystemContext context)
        {
            _context = context;
        }

        public async Task<List<Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Report> GetReportByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public async Task<List<Report>> GetReportsByCustomerAsync(int customerId)
        {
            return await _context.Reports
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<List<Report>> GetReportsByLoginAsync(int userId)
        {
            return await _context.Reports
                .Where(r => r.CustomerId == userId)
                .ToListAsync();
        }

        public async Task<Report> CreateReportAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }


        public async Task<Report> UpdateReportAsync(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                return false;
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
