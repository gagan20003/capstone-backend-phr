using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Services
{
    public interface IAppointmentService
    {
        Task<List<Appointments>> GetForUserAsync(string userId);
        Task<Appointments?> GetByIdForUserAsync(string userId, int appointmentId);
        Task<Appointments> CreateForUserAsync(string userId, CreateUpdateAppointmentDto dto);
        Task<Appointments?> UpdateForUserAsync(string userId, int appointmentId, CreateUpdateAppointmentDto dto);
        Task<bool> DeleteForUserAsync(string userId, int appointmentId);
    }
}