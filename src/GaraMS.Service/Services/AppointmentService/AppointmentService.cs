using GaraMS.Data.Models;
using GaraMS.Data.Repositories.AppointmentRepo;
using GaraMS.Data.ViewModels.AppointmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.AppointmentService
{
	public class AppointmentService : IAppointmentService
	{
		private readonly IAppointmentRepo _appointmentRepo;

		public AppointmentService(IAppointmentRepo appointmentRepo)
		{
			_appointmentRepo = appointmentRepo;
		}
		public async Task<Appointment> CreateAppointmentAsync(AppointmentDTO DTO)
		{
			return await _appointmentRepo.CreateAppointmentAsync(DTO);
		}

		public async Task<bool> DeleteAppointmentAsync(int id)
		{
			return await _appointmentRepo.DeleteAppointmentAsync(id);
		}

		public async Task<List<Appointment>> GetAllAppointmentsAsync()
		{
			return await _appointmentRepo.GetAllAppointmentsAsync();
		}

		public async Task<Appointment> GetAppointmentByIdAsync(int id)
		{
			return await _appointmentRepo.GetAppointmentByIdAsync(id);
		}

		public async Task<bool> UpdateAppointmentAsync(int id, AppointmentDTO DTO)
		{
			return await _appointmentRepo.UpdateAppointmentAsync(id, DTO);
		}
	}
}
