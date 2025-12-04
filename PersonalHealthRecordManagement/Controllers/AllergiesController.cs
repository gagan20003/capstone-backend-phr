using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/allergies")]
    public class AllergiesController : BaseController
    {
        private readonly IAllergyService _allergyService;
        private readonly ILogger<AllergiesController> _logger;

        public AllergiesController(IAllergyService allergyService, ILogger<AllergiesController> logger)
        {
            _allergyService = allergyService;
            _logger = logger;
        }

        /// <summary>
        /// Get all allergies for the logged-in user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Allergies>>> GetMyAllergies()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<List<Allergies>>();

            var allergies = await _allergyService.GetForUserAsync(userId);
            return Ok(allergies);
        }

        /// <summary>
        /// Get details of a specific allergy
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Allergies>> GetMyAllergy(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Allergies>();

            var allergy = await _allergyService.GetByIdForUserAsync(userId, id);
            if (allergy == null) return NotFoundResponse<Allergies>("Allergy not found");

            return Ok(allergy);
        }

        /// <summary>
        /// Add a new allergy
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Allergies>> CreateMyAllergy([FromBody] AllergyCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Allergies>();

            try
            {
                var created = await _allergyService.CreateForUserAsync(userId, dto);
                _logger.LogInformation("Allergy created: AllergyId={AllergyId}, UserId={UserId}", created.AllergyId, userId);
                return CreatedAtAction(nameof(GetMyAllergy), new { id = created.AllergyId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating allergy for UserId={UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Update allergy details
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Allergies>> UpdateMyAllergy(int id, [FromBody] AllergyCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Allergies>();

            var updated = await _allergyService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFoundResponse<Allergies>("Allergy not found");

            _logger.LogInformation("Allergy updated: AllergyId={AllergyId}, UserId={UserId}", id, userId);
            return Ok(updated);
        }

        /// <summary>
        /// Delete an allergy
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMyAllergy(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _allergyService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();

            _logger.LogInformation("Allergy deleted: AllergyId={AllergyId}, UserId={UserId}", id, userId);
            return NoContent();
        }
    }
}
