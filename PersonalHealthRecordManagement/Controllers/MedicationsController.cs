using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/medications")]
    public class MedicationsController : BaseController
    {
        private readonly IMedicationService _medicationService;
        private readonly ILogger<MedicationsController> _logger;

        public MedicationsController(IMedicationService medicationService, ILogger<MedicationsController> logger)
        {
            _medicationService = medicationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all medications for the logged-in user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Medications>>> GetMyMedications()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<List<Medications>>();

            var meds = await _medicationService.GetForUserAsync(userId);
            return Ok(meds);
        }

        /// <summary>
        /// Get details of a specific medication
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Medications>> GetMyMedication(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Medications>();

            var med = await _medicationService.GetByIdForUserAsync(userId, id);
            if (med == null) return NotFoundResponse<Medications>("Medication not found");

            return Ok(med);
        }

        /// <summary>
        /// Add a new medication
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Medications>> CreateMyMedication([FromBody] MedicationCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Medications>();

            try
            {
                var created = await _medicationService.CreateForUserAsync(userId, dto);
                _logger.LogInformation("Medication created: MedicationId={MedicationId}, UserId={UserId}", created.MedicationId, userId);
                return CreatedAtAction(nameof(GetMyMedication), new { id = created.MedicationId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medication for UserId={UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Update medication details
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Medications>> UpdateMyMedication(int id, [FromBody] MedicationCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Medications>();

            var updated = await _medicationService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFoundResponse<Medications>("Medication not found");

            _logger.LogInformation("Medication updated: MedicationId={MedicationId}, UserId={UserId}", id, userId);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a medication
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMyMedication(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _medicationService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();

            _logger.LogInformation("Medication deleted: MedicationId={MedicationId}, UserId={UserId}", id, userId);
            return NoContent();
        }
    }
}