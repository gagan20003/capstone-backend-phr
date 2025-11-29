using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public interface IMedicationRepository
    {
        Task<List<Medications>> GetByUserProfileIdAsync(int userProfileId);
        Task<Medications?> GetByIdAsync(int medicationId);
        Task AddAsync(Medications medication);
        Task UpdateAsync(Medications medication);
        Task DeleteAsync(Medications medication);
        Task SaveChangesAsync();

    }
}
