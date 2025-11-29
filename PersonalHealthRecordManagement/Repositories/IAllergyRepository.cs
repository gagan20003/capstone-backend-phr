using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public interface IAllergyRepository
    {
        Task<List<Allergies>> GetByUserProfileIdAsync(int userProfileId);
        Task<Allergies?> GetByIdAsync(int allergyId);
        Task AddAsync(Allergies allergy);
        Task UpdateAsync(Allergies allergy);
        Task DeleteAsync(Allergies allergy);
        Task SaveChangesAsync();
    }
}
