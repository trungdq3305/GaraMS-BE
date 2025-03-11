using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentModel;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.AppointmentService
{
	public interface IAppointmentService
	{
		Task<ResultModel> GetAllAppointmentsAsync(string? token);
		Task<ResultModel> GetAppointmentByIdAsync(string? token, int id);
		Task<ResultModel> CreateAppointmentAsync(string? token, AppointmentModel model);
		Task<ResultModel> UpdateAppointmentAsync(string? token, int id, AppointmentModel model);
		Task<ResultModel> UpdateAppointmentStatusAsync(string? token, int id, string status, string reason);
		Task<ResultModel> DeleteAppointmentAsync(string? token, int id);
	}
}
