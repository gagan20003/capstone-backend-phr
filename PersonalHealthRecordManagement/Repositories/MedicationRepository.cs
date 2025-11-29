using Microsoft.EntityFrameworkCore;
using PersonalHealthRecordManagement.Data;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly AppDbContext _context;
        public MedicationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Medications>> GetByUserProfileIdAsync(int userProfileId)
        {
            return await _context.Medications
            .Where(m => m.UserProfileId == userProfileId)
            .ToListAsync();
        }
        public async Task<Medications?> GetByIdAsync(int medicationId)
        {
            return await _context.Medications
            .Include(m => m.UserProfile)
            .FirstOrDefaultAsync(m => m.MedicationId == medicationId);
        }
        public async Task AddAsync(Medications medication)
        {
            await _context.Medications.AddAsync(medication);
        }
        public Task UpdateAsync(Medications medication)
        {
            _context.Medications.Update(medication);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Medications medication)
        {
            _context.Medications.Remove(medication);
            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}