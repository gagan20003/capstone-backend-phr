using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Repositories;

namespace PersonalHealthRecordManagement.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<Appointments>> GetForUserAsync(string userId)
        {
            return await _appointmentRepository.GetByUserIdAsync(userId);
        }

        public async Task<Appointments?> GetByIdForUserAsync(string userId, int appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null || appointment.UserId != userId)
            {
                return null;
            }

            return appointment;
        }

        public async Task<Appointments> CreateForUserAsync(string userId, CreateUpdateAppointmentDto dto)
        {
            var appointment = new Appointments
            {
                UserId = userId,
                DoctorName = dto.DoctorName,
                Purpose = dto.Purpose,
                AppointmentDate = dto.AppointmentDate,
                Status = dto.Status ?? "Scheduled",
                CreatedAt = DateTime.UtcNow
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return appointment;
        }

        public async Task<Appointments?> UpdateForUserAsync(string userId, int appointmentId, CreateUpdateAppointmentDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null || appointment.UserId != userId)
            {
                return null;
            }

            appointment.DoctorName = dto.DoctorName;
            appointment.Purpose = dto.Purpose;
            appointment.AppointmentDate = dto.AppointmentDate;

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                appointment.Status = dto.Status;
            }

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return appointment;
        }

        public async Task<bool> DeleteForUserAsync(string userId, int appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null || appointment.UserId != userId)
            {
                return false;
            }

            await _appointmentRepository.DeleteAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return true;
        }
    }
}