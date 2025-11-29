using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Services
{
    public interface IAllergyService
    {
        Task<List<Allergies>> GetForUserAsync(string userId);
        Task<Allergies?> GetByIdForUserAsync(string userId, int allergyId);
        Task<Allergies> CreateForUserAsync(string userId, AllergyCreateUpdateDto dto);
        Task<Allergies?> UpdateForUserAsync(string userId, int allergyId, AllergyCreateUpdateDto
       dto);
        Task<bool> DeleteForUserAsync(string userId, int allergyId);
    }
}
