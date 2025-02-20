using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentDTO;
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
		Task<Appointment> GetAppointmentByIdAsync(int id);
		Task<Appointment> CreateAppointmentAsync(AppointmentDTO dto);
		Task<bool> UpdateAppointmentAsync(int id, AppointmentDTO dto);
		Task<bool> DeleteAppointmentAsync(int id);
	}
}
