using GaraMS.Data.Models;
using GaraMS.Data.Repositories.AppointmentRepo;
using GaraMS.Data.ViewModels.AppointmentModel;
using GaraMS.Data.ViewModels.ResultModel;
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

		public async Task<ResultModel> CreateAppointmentAsync(string token, AppointmentModel model)
		{
			var appointment = await _appointmentRepo.CreateAppointmentAsync(model);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to create appointment" };

			return new ResultModel { IsSuccess = true, Code = 201, Data = appointment, Message = "Appointment created successfully" };
		}

		public async Task<ResultModel> DeleteAppointmentAsync(string token, int id)
		{
			var appointment = await _appointmentRepo.DeleteAppointmentAsync(id);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to delete appointment" };

			return new ResultModel { IsSuccess = true, Code = 200, Message = "Appointment deleted successfully" };
		}

		public async Task<ResultModel> GetAllAppointmentsAsync()
		{
			var appointments = await _appointmentRepo.GetAllAppointmentsAsync();
			return new ResultModel { IsSuccess = true, Code = 200, Data = appointments, Message = "Appointments retrieved successfully" };
		}

		public async Task<ResultModel> GetAppointmentByIdAsync(int id)
		{
			var appointment = await _appointmentRepo.GetAppointmentByIdAsync(id);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 404, Message = "Appointment not found" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = appointment, Message = "Appointment retrieved successfully" };
		}

		public async Task<ResultModel> UpdateAppointmentAsync(string token, int id, AppointmentModel model)
		{
			var appointment = await _appointmentRepo.UpdateAppointmentAsync(id, model);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update appointment" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = appointment, Message = "Appointment updated successfully" };
		}

		public async Task<ResultModel> UpdateAppointmentStatusAsync(string token, int id, string status, string reason)
		{
			var appointment = await _appointmentRepo.UpdateAppointmentStatusAsync(id, status, reason);
			if (appointment == null)
				return new ResultModel { IsSuccess = false, Code = 400, Message = "Failed to update appointment status" };

			return new ResultModel { IsSuccess = true, Code = 200, Data = appointment, Message = "Appointment status updated successfully" };
		}
	}
}
