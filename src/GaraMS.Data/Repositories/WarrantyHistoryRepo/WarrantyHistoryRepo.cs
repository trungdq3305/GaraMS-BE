using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.WarrantyHistoryModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.WarrantyHistoryRepo
{
    public class WarrantyHistoryRepo : IWarrantyHistoryRepo
	{
		private readonly GaraManagementSystemContext _context;

		public WarrantyHistoryRepo(GaraManagementSystemContext context) 
        {
			_context = context;
        }

		public async Task<WarrantyHistory> CreateWarrantyHistoryAsync(WarrantyHistoryModel model)
		{
			var warranty = new WarrantyHistory
			{
				Note = model.Note,
				Status = model.Status,
				ServiceId = model.ServiceId
			};

			_context.WarrantyHistories.Add(warranty);
			await _context.SaveChangesAsync();
			return warranty;
		}

		public async Task<WarrantyHistory> DeleteWarrantyHistoryAsync(int id)
		{
			var warranty = await _context.WarrantyHistories.FindAsync(id);
			if (warranty == null) return null;

			_context.WarrantyHistories.Remove(warranty);
			await _context.SaveChangesAsync();
			return warranty;
		}

		public async Task<List<WarrantyHistoryModel>> GetAllWarrantyHistoriesAsync()
		{
			return await _context.WarrantyHistories
				.Select(w => new WarrantyHistoryModel
				{
					WarrantyHistoryId = w.WarrantyHistoryId,
					StartDay = w.StartDay,
					EndDay = w.EndDay,
					Note = w.Note,
					Status = w.Status,
					ServiceId = w.ServiceId
				}).ToListAsync();
		}

		public async Task<WarrantyHistory> GetWarrantyHistoryByIdAsync(int id)
		{
			return await _context.WarrantyHistories.FindAsync(id);
		}

		public async Task<WarrantyHistory> UpdateWarrantyHistoryAsync(int id, WarrantyHistoryModel model)
		{
			var warranty = await _context.WarrantyHistories.FindAsync(id);
			if (warranty == null) return null;

			warranty.StartDay = model.StartDay;
			warranty.EndDay = model.EndDay;

			_context.WarrantyHistories.Update(warranty);
			await _context.SaveChangesAsync();
			return warranty;
		}
	}
}
