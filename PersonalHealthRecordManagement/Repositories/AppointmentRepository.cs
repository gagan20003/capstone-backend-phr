using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalHealthRecordManagement.Data;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointments>> GetByUserIdAsync(string userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.AppointmentDate)
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task<Appointments?> GetByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task AddAsync(Appointments appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public Task UpdateAsync(Appointments appointment)
        {
            _context.Appointments.Update(appointment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Appointments appointment)
        {
            _context.Appointments.Remove(appointment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}