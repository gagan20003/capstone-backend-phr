using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile?> GetForUserAsync(string userId);

        Task<UserProfile> UpsertForUserAsync(string userId, UpdateUserProfileDto dto);
    }
}
