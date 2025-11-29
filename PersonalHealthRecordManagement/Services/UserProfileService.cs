using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Repositories;

namespace PersonalHealthRecordManagement.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileService(
            IUserProfileRepository userProfileRepository,
            UserManager<ApplicationUser> userManager)
        {
            _userProfileRepository = userProfileRepository;
            _userManager = userManager;
        }

        public async Task<UserProfile?> GetForUserAsync(string userId)
        {
            return await _userProfileRepository.GetByUserIdAsync(userId);
        }

        public async Task<UserProfile> UpsertForUserAsync(string userId, UpdateUserProfileDto dto)
        {
            // Ensure user exists
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var existingProfile = await _userProfileRepository.GetByUserIdAsync(userId);

            if (existingProfile == null)
            {
                // Create new profile
                var profile = new UserProfile
                {
                    UserId = userId,
                    Age = dto.Age,
                    Gender = dto.Gender,
                    Weight = dto.Weight,
                    BloodGroup = dto.BloodGroup,
                    Emergencycontact = dto.Emergencycontact
                };

                await _userProfileRepository.AddAsync(profile);
                await _userProfileRepository.SaveChangesAsync();
                return profile;
            }
            else
            {
                // Update existing profile
                existingProfile.Age = dto.Age;
                existingProfile.Gender = dto.Gender;
                existingProfile.Weight = dto.Weight;
                existingProfile.BloodGroup = dto.BloodGroup;
                existingProfile.Emergencycontact = dto.Emergencycontact;

                await _userProfileRepository.UpdateAsync(existingProfile);
                await _userProfileRepository.SaveChangesAsync();
                return existingProfile;
            }
        }
    }
}