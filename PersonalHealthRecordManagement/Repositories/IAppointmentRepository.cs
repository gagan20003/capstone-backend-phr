using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{ 
    public interface IAppointmentRepository
    {
        Task<List<Appointments>> GetByUserIdAsync(string userId);
        Task<Appointments?> GetByIdAsync(int appointmentId);
        Task AddAsync(Appointments appointment);
        Task UpdateAsync(Appointments appointment);
        Task DeleteAsync(Appointments appointment);
        Task SaveChangesAsync();
    }
}
