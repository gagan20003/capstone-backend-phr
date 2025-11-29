using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/medications")]
    [Authorize]
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationService _medicationService;
        public MedicationsController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        [HttpGet]
        public async Task<ActionResult<List<Medications>>> GetMyMedications()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var meds = await _medicationService.GetForUserAsync(userId);
            return Ok(meds);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Medications>> GetMyMedication(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var med = await _medicationService.GetByIdForUserAsync(userId, id);
            if (med == null) return NotFound();
            return Ok(med);
        }
        [HttpPost]
        public async Task<ActionResult<Medications>> CreateMyMedication([FromBody]
MedicationCreateUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var created = await _medicationService.CreateForUserAsync(userId, dto);
            return CreatedAtAction(nameof(GetMyMedication), new { id = created.MedicationId },
           created);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Medications>> UpdateMyMedication(int id, [FromBody]
MedicationCreateUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var updated = await _medicationService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMyMedication(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var success = await _medicationService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}