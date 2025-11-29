using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Services
{
    public interface IMedicationService
    {
        Task<List<Medications>> GetForUserAsync(string userId);
        Task<Medications?> GetByIdForUserAsync(string userId, int medicationId);
        Task<Medications> CreateForUserAsync(string userId, MedicationCreateUpdateDto dto);
        Task<Medications?> UpdateForUserAsync(string userId, int medicationId,
       MedicationCreateUpdateDto dto);
        Task<bool> DeleteForUserAsync(string userId, int medicationId);
    }
}
