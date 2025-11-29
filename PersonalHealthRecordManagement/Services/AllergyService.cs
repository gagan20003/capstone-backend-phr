using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Repositories;

namespace PersonalHealthRecordManagement.Services
{
    public class AllergyService : IAllergyService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAllergyRepository _allergyRepository;
        public AllergyService(
        IUserProfileRepository userProfileRepository,
        IAllergyRepository allergyRepository)
        {
            _userProfileRepository = userProfileRepository;
            _allergyRepository = allergyRepository;
        }
        public async Task<List<Allergies>> GetForUserAsync(string userId)
        {
            var profile = await EnsureUserProfileAsync(userId);
            return await _allergyRepository.GetByUserProfileIdAsync(profile.UserProfileId);
        }
        public async Task<Allergies?> GetByIdForUserAsync(string userId, int allergyId)
        {
            var allergy = await _allergyRepository.GetByIdAsync(allergyId);
            if (allergy == null || allergy.UserProfile.UserId != userId)
            {
                return null;
            }
            return allergy;
        }
        public async Task<Allergies> CreateForUserAsync(string userId, AllergyCreateUpdateDto dto)
        {
            var profile = await EnsureUserProfileAsync(userId);
            var allergy = new Allergies
            {
                UserProfileId = profile.UserProfileId,
                AllergyName = dto.AllergyName,
                Symptoms = dto.Symptoms,
                Severity = dto.Severity
            };
            await _allergyRepository.AddAsync(allergy);
            await _allergyRepository.SaveChangesAsync();
            return allergy;
        }
        public async Task<Allergies?> UpdateForUserAsync(string userId, int allergyId,
       AllergyCreateUpdateDto dto)
        {
            var allergy = await _allergyRepository.GetByIdAsync(allergyId);
            if (allergy == null || allergy.UserProfile.UserId != userId)
            {
                return null;
            }
            allergy.AllergyName = dto.AllergyName;
            allergy.Symptoms = dto.Symptoms;
            allergy.Severity = dto.Severity;
            await _allergyRepository.UpdateAsync(allergy);
            await _allergyRepository.SaveChangesAsync();
            return allergy;
        }
        public async Task<bool> DeleteForUserAsync(string userId, int allergyId)
        {
            var allergy = await _allergyRepository.GetByIdAsync(allergyId);
            if (allergy == null || allergy.UserProfile.UserId != userId)
            {
                return false;
            }
            await _allergyRepository.DeleteAsync(allergy);
            await _allergyRepository.SaveChangesAsync();
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