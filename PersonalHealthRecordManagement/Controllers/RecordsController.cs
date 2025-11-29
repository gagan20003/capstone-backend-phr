using System.Collections.Generic;
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
    [Route("api/records")]
    [Authorize] // all endpoints require authenticated user
    public class RecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public RecordsController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET /api/records
        // Get all records for logged-in user
        [HttpGet]
        public async Task<ActionResult<List<MedicalRecords>>> GetMyRecords()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var records = await _medicalRecordService.GetForUserAsync(userId);
            return Ok(records);
        }

        // GET /api/records/{id}
        // Get details of a specific record
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MedicalRecords>> GetMyRecord(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var record = await _medicalRecordService.GetByIdForUserAsync(userId, id);
            if (record == null) return NotFound();

            return Ok(record);
        }

        // POST /api/records
        // Add a new medical record
        [HttpPost]
        public async Task<ActionResult<MedicalRecords>> CreateRecord([FromBody] CreateUpdateMedicalRecordDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var created = await _medicalRecordService.CreateForUserAsync(userId, dto);
            return CreatedAtAction(nameof(GetMyRecord), new { id = created.RecordId }, created);
        }

        // PUT /api/records/{id}
        // Update record details
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MedicalRecords>> UpdateRecord(int id, [FromBody] CreateUpdateMedicalRecordDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updated = await _medicalRecordService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE /api/records/{id}
        // Delete a record
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRecord(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _medicalRecordService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}