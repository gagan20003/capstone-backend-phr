using Microsoft.EntityFrameworkCore;
using PersonalHealthRecordManagement.Data;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;

        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.UserProfiles
                .Include(p => p.Allergies)
                .Include(p => p.Medications)
                .Include(p=> p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
        public async Task<UserProfile?> GetByIdAsync(int id)
        {
            return await _context.UserProfiles
                .Include(p => p.Allergies)
                .Include(p => p.Medications)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserProfileId == id);
        }

        public async Task AddAsync(UserProfile profile)
        {
            await _context.UserProfiles.AddAsync(profile);
        }

        public Task UpdateAsync(UserProfile profile) {
            _context.UserProfiles.Update(profile);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
