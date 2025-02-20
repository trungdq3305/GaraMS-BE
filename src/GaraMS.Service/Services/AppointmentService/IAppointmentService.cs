using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.AppointmentService
{
	public interface IAppointmentService
	{
		Task<List<Appointment>> GetAllAppointmentsAsync();
		Task<Appointment> GetAppointmentByIdAsync(int id);
		Task<Appointment> CreateAppointmentAsync(AppointmentDTO DTO);
        Task<bool> UpdateAppointmentStatusAsync(int id, string status, string reason);
        Task<bool> UpdateAppointmentAsync(int id, AppointmentDTO DTO);
		Task<bool> DeleteAppointmentAsync(int id);
	}
}
