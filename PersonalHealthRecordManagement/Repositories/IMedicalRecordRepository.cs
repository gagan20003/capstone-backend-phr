using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public interface IMedicalRecordRepository
    {
        Task<List<MedicalRecords>> GetByUserIdAsync(string userId);
        Task<MedicalRecords?> GetByIdAsync(int recordId);
        Task AddAsync(MedicalRecords record);
        Task UpdateAsync(MedicalRecords record);
        Task DeleteAsync(MedicalRecords record);
        Task SaveChangesAsync();

    }
}
