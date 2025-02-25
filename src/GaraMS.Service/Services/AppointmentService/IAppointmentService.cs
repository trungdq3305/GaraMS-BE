using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentDTO;
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
		Task<ResultModel> GetAllAppointmentsAsync();
		Task<ResultModel> GetAppointmentByIdAsync(int id);
		Task<ResultModel> CreateAppointmentAsync(string token, AppointmentDTO DTO);
		Task<ResultModel> UpdateAppointmentAsync(string token, int id, AppointmentDTO dto);
		Task<ResultModel> UpdateAppointmentStatusAsync(string token, int id, string status, string reason);
		Task<ResultModel> DeleteAppointmentAsync(string token, int id);
	}
}
