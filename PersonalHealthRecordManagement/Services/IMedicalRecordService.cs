using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Services
{
    public interface IMedicalRecordService
    {
        Task<List<MedicalRecords>> GetForUserAsync(string userId);
        Task<MedicalRecords?> GetByIdForUserAsync(string userId, int recordId);
        Task<MedicalRecords> CreateForUserAsync(string userId, CreateUpdateMedicalRecordDto dto);
        Task<MedicalRecords?> UpdateForUserAsync(string userId, int recordId, CreateUpdateMedicalRecordDto dto);
        Task<bool> DeleteForUserAsync(string userId, int recordId);
    }
}