using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // require authenticated user
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Get the profile for the currently logged-in user.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<UserProfile>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var profile = await _userProfileService.GetForUserAsync(userId);

            if (profile == null)
            {
                return NotFound("Profile not found for current user.");
            }

            return Ok(profile);
        }

        /// <summary>
        /// Create or update the profile for the currently logged-in user.
        /// </summary>
        [HttpPut("me")]
        public async Task<ActionResult<UserProfile>> UpsertMyProfile([FromBody] UpdateUserProfileDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var profile = await _userProfileService.UpsertForUserAsync(userId, dto);
            return Ok(profile);
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}