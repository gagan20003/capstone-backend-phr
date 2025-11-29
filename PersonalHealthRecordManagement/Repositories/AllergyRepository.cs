using Microsoft.EntityFrameworkCore;
using PersonalHealthRecordManagement.Data;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public class AllergyRepository : IAllergyRepository
    {
        private readonly AppDbContext _context;
        public AllergyRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Allergies>> GetByUserProfileIdAsync(int userProfileId)
        {
            return await _context.Allergies
            .Where(a => a.UserProfileId == userProfileId)
            .ToListAsync();
        }
        public async Task<Allergies?> GetByIdAsync(int allergyId)
        {
            return await _context.Allergies
            .Include(a => a.UserProfile)
            .FirstOrDefaultAsync(a => a.AllergyId == allergyId);
        }
        public async Task AddAsync(Allergies allergy)
        {
            await _context.Allergies.AddAsync(allergy);
        }
        public Task UpdateAsync(Allergies allergy)
        {
            _context.Allergies.Update(allergy);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Allergies allergy)
        {
            _context.Allergies.Remove(allergy);
            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}