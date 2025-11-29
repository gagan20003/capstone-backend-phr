using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Repositories;

namespace PersonalHealthRecordManagement.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMedicationRepository _medicationRepository;
        public MedicationService(
        IUserProfileRepository userProfileRepository,
        IMedicationRepository medicationRepository)
        {
            _userProfileRepository = userProfileRepository;
            _medicationRepository = medicationRepository;
        }
        public async Task<List<Medications>> GetForUserAsync(string userId)
        {
            var profile = await EnsureUserProfileAsync(userId);
            return await _medicationRepository.GetByUserProfileIdAsync(profile.UserProfileId);
        }
        public async Task<Medications?> GetByIdForUserAsync(string userId, int medicationId)
        {
            var med = await _medicationRepository.GetByIdAsync(medicationId);
            if (med == null || med.UserProfile.UserId != userId)
            {
                return null;
            }
            return med;
        }
        public async Task<Medications> CreateForUserAsync(string userId, MedicationCreateUpdateDto
       dto)
        {
            var profile = await EnsureUserProfileAsync(userId);
            var med = new Medications
            {
                UserProfileId = profile.UserProfileId,
                MedicineName = dto.MedicineName,
                Quantity = dto.Quantity,
                Frequency = dto.Frequency,
                PrescribedFor = dto.PrescribedFor,
                PrescribedBy = dto.PrescribedBy,
                DatePrescribed = dto.DatePrescribed.HasValue
            ? DateOnly.FromDateTime(dto.DatePrescribed.Value)
            : (DateOnly?)null
            };
            await _medicationRepository.AddAsync(med);
            await _medicationRepository.SaveChangesAsync();
            return med;
        }
        public async Task<Medications?> UpdateForUserAsync(string userId, int medicationId,
       MedicationCreateUpdateDto dto)
        {
            var med = await _medicationRepository.GetByIdAsync(medicationId);
            if (med == null || med.UserProfile.UserId != userId)
            {
                return null;
            }
            med.MedicineName = dto.MedicineName;
            med.Quantity = dto.Quantity;
            med.Frequency = dto.Frequency;
            med.PrescribedFor = dto.PrescribedFor;
            med.PrescribedBy = dto.PrescribedBy;
            med.DatePrescribed = dto.DatePrescribed.HasValue
            ? DateOnly.FromDateTime(dto.DatePrescribed.Value)
            : (DateOnly?)null;
            await _medicationRepository.UpdateAsync(med);
            await _medicationRepository.SaveChangesAsync();
            return med;
        }
        public async Task<bool> DeleteForUserAsync(string userId, int medicationId)
        {
            var med = await _medicationRepository.GetByIdAsync(medicationId);
            if (med == null || med.UserProfile.UserId != userId)
            {
                return false;
            }
            await _medicationRepository.DeleteAsync(med);
            await _medicationRepository.SaveChangesAsync();
            return true;
        }
        private async Task<UserProfile> EnsureUserProfileAsync(string userId)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (profile != null) return profile;
            var newProfile = new UserProfile
            {
                UserId = userId
            };
            await _userProfileRepository.AddAsync(newProfile);
            await _userProfileRepository.SaveChangesAsync();
            return newProfile;
        }
    }
}