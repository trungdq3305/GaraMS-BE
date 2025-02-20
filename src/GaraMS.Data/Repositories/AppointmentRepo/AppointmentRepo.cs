using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.Repositories.AppointmentRepo
{
	public class AppointmentRepo : IAppointmentRepo
	{
		private readonly GaraManagementSystemContext _context;

		public AppointmentRepo(GaraManagementSystemContext context)
		{
			_context = context;
		}
		public async Task<Appointment> CreateAppointmentAsync(AppointmentDTO dto)
		{
			var appointment = new Appointment
			{
				Date = dto.Date,
				Note = dto.Note,
				Status = "Pending",
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				VehicleId = dto.VehicleId
			};

			_context.Appointments.Add(appointment);
			await _context.SaveChangesAsync();

			// Adding services to the appointment
			if (dto.ServiceIds.Any())
			{
				foreach (var serviceId in dto.ServiceIds)
				{
					_context.AppointmentServices.Add(new AppointmentService
					{
						AppointmentId = appointment.AppointmentId,
						ServiceId = serviceId
					});
				}
				await _context.SaveChangesAsync();
			}
			return appointment;
		}

		public async Task<bool> DeleteAppointmentAsync(int id)
		{
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment == null) return false;

			_context.Appointments.Remove(appointment);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<Appointment>> GetAllAppointmentsAsync()
		{
			return await _context.Appointments
				.Include(a => a.Vehicle)
				.Include(a => a.AppointmentServices)
				.ThenInclude(asv => asv.Service)
				.ToListAsync();
		}

		public async Task<Appointment> GetAppointmentByIdAsync(int id)
		{
			return await _context.Appointments
				.Include(a => a.Vehicle)
				.Include(a => a.AppointmentServices)
				.ThenInclude(asv => asv.Service)
				.FirstOrDefaultAsync(a => a.AppointmentId == id);
		}

		public async Task<bool> UpdateAppointmentAsync(int id, AppointmentDTO dto)
		{
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment == null) return false;

			appointment.Date = dto.Date;
			appointment.Note = dto.Note;
			appointment.UpdatedAt = DateTime.UtcNow;
			appointment.VehicleId = dto.VehicleId;

			_context.Appointments.Update(appointment);
			await _context.SaveChangesAsync();
			return true;
		}
        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status, string reason)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;
            appointment.UpdatedAt = DateTime.UtcNow;
			if(status == "Accept")
			{
				appointment.Status = "Accept";
			}
            if (status == "Reject")
            {
                appointment.Status = "Reject";
				appointment.RejectReason = reason;
            }
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
