using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : BaseController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileService userProfileService, ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _logger = logger;
        }

        /// <summary>
        /// Get the profile for the currently logged-in user
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<UserProfile>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<UserProfile>();

            var profile = await _userProfileService.GetForUserAsync(userId);
            if (profile == null) return NotFoundResponse<UserProfile>("Profile not found for current user");

            return Ok(profile);
        }

        /// <summary>
        /// Create or update the profile for the currently logged-in user
        /// </summary>
        [HttpPut("me")]
        public async Task<ActionResult<UserProfile>> UpsertMyProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<UserProfile>();

            try
            {
                var profile = await _userProfileService.UpsertForUserAsync(userId, dto);
                _logger.LogInformation("User profile upserted: UserId={UserId}", userId);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while upserting profile: UserId={UserId}", userId);
                return BadRequestResponse<UserProfile>(ex.Message);
            }
        }
    }
}