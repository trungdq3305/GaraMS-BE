using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.AppointmentRepo
{
	public interface IAppointmentRepo
	{
		Task<List<Appointment>> GetAllAppointmentsAsync();
		Task<List<Appointment>> GetAppointmentsByUserId(int customerid);
        Task<Appointment> GetAppointmentByIdAsync(int id);
		Task<Appointment> CreateAppointmentAsync(AppointmentModel model);
		Task<Appointment?> UpdateAppointmentAsync(int id, AppointmentModel model);
		Task<Appointment?> UpdateAppointmentStatusAsync(int id, string status, string reason);
		Task<Appointment?> DeleteAppointmentAsync(int id);
	}
}
