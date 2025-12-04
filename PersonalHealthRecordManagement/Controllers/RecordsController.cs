using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/records")]
    public class RecordsController : BaseController
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly ILogger<RecordsController> _logger;

        public RecordsController(IMedicalRecordService medicalRecordService, ILogger<RecordsController> logger)
        {
            _medicalRecordService = medicalRecordService;
            _logger = logger;
        }

        /// <summary>
        /// Get all medical records for the logged-in user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<MedicalRecords>>> GetMyRecords()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<List<MedicalRecords>>();

            var records = await _medicalRecordService.GetForUserAsync(userId);
            return Ok(records);
        }

        /// <summary>
        /// Get details of a specific medical record
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MedicalRecords>> GetMyRecord(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<MedicalRecords>();

            var record = await _medicalRecordService.GetByIdForUserAsync(userId, id);
            if (record == null) return NotFoundResponse<MedicalRecords>("Medical record not found");

            return Ok(record);
        }

        /// <summary>
        /// Add a new medical record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MedicalRecords>> CreateRecord([FromBody] CreateUpdateMedicalRecordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<MedicalRecords>();

            try
            {
                var created = await _medicalRecordService.CreateForUserAsync(userId, dto);
                _logger.LogInformation("Medical record created: RecordId={RecordId}, UserId={UserId}", created.RecordId, userId);
                return CreatedAtAction(nameof(GetMyRecord), new { id = created.RecordId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical record for UserId={UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Update medical record details
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MedicalRecords>> UpdateRecord(int id, [FromBody] CreateUpdateMedicalRecordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<MedicalRecords>();

            var updated = await _medicalRecordService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFoundResponse<MedicalRecords>("Medical record not found");

            _logger.LogInformation("Medical record updated: RecordId={RecordId}, UserId={UserId}", id, userId);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a medical record
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _medicalRecordService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();

            _logger.LogInformation("Medical record deleted: RecordId={RecordId}, UserId={UserId}", id, userId);
            return NoContent();
        }
    }
}