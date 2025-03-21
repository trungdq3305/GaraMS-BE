using GaraMS.Data.Models;
using GaraMS.Data.ViewModels.AppointmentModel;
using GaraMS.Data.ViewModels.AppointmentModel;
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

		public async Task<Appointment> CreateAppointmentAsync(AppointmentModel model)
		{
			var appointment = new Appointment
			{
				Date = model.Date,
				Note = model.Note,
				Status = "Pending",
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				VehicleId = model.VehicleId
			};

			_context.Appointments.Add(appointment);
			await _context.SaveChangesAsync();

			// Adding services to the appointment
			if (model.ServiceIds != null && model.ServiceIds.Any())
			{
				foreach (var serviceId in model.ServiceIds)
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

		public async Task<Appointment> DeleteAppointmentAsync(int id)
		{
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment == null) return null;

			_context.Appointments.Remove(appointment);
			await _context.SaveChangesAsync();
			return appointment;
		}

		public async Task<List<Appointment>> GetAllAppointmentsAsync()
		{
			return await _context.Appointments
				.Include(a => a.Vehicle)
				.ThenInclude(q=>q.Customer).ThenInclude(q => q.User)
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

        public async Task<List<Appointment>> GetAppointmentsByUserId(int customerid)
        {
            return await _context.Appointments
                .Include(a => a.Vehicle)
                .Include(a => a.AppointmentServices)
                .ThenInclude(asv => asv.Service)
                .Where(a => a.Vehicle.CustomerId == customerid)
                .ToListAsync();
        }

        public async Task<Appointment> UpdateAppointmentAsync(int id, AppointmentModel model)
		{
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment == null) return null;

			appointment.Date = model.Date;
			appointment.Note = model.Note;
			appointment.UpdatedAt = DateTime.UtcNow;
			appointment.VehicleId = model.VehicleId;

			_context.Appointments.Update(appointment);
			await _context.SaveChangesAsync();
			return appointment;
		}

		public async Task<Appointment> UpdateAppointmentStatusAsync(int id, string status, string reason)
		{
			var appointment = await _context.Appointments
				.Include(a => a.AppointmentServices)
				.ThenInclude(asv => asv.Service)
				.FirstOrDefaultAsync(a => a.AppointmentId == id);

			if (appointment == null) return null;

			appointment.UpdatedAt = DateTime.UtcNow;

			if (status.Equals("Accept", StringComparison.OrdinalIgnoreCase))
			{
				appointment.Status = "Accept";
			}
			else if (status.Equals("Reject", StringComparison.OrdinalIgnoreCase))
			{
				appointment.Status = "Reject";
				appointment.RejectReason = reason;
			}
			else if (status.Equals("Complete", StringComparison.OrdinalIgnoreCase))
			{
				appointment.Status = "Complete";
				appointment.RejectReason = reason;

				// First save the appointment changes
				_context.Appointments.Update(appointment);
				await _context.SaveChangesAsync();

				// Then create warranty histories
				foreach (var appointmentService in appointment.AppointmentServices)
				{
					var service = appointmentService.Service;
					if (service != null && service.WarrantyPeriod.HasValue)
					{
						int warrantyPeriod = (int)service.WarrantyPeriod;

						var warrantyHistory = new WarrantyHistory
						{
							ServiceId = service.ServiceId,
							StartDay = DateTime.UtcNow,
							EndDay = DateTime.UtcNow.AddDays(warrantyPeriod - 1),
							Status = true,
							Note = $"AppointmentID: {id}, ServiceID: {service.ServiceId}"
						};

						_context.WarrantyHistories.Add(warrantyHistory);
					}
				}
				// Save warranty histories separately
				await _context.SaveChangesAsync();
				
				return appointment;
			}
			else
			{
				return null;
			}

			_context.Appointments.Update(appointment);
			await _context.SaveChangesAsync();
			return appointment;
		}
	}
}
