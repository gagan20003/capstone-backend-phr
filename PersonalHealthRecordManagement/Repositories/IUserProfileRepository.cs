

using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<UserProfile?> GetByIdAsync(int id);
        Task AddAsync(UserProfile profile);
        Task UpdateAsync(UserProfile profile);

        Task SaveChangesAsync();
    }
}
