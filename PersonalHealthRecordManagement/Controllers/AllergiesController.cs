using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/allergies")]
    [Authorize]
    public class AllergiesController : ControllerBase
    {
        private readonly IAllergyService _allergyService;
        public AllergiesController(IAllergyService allergyService)
        {
            _allergyService = allergyService;
        }
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        [HttpGet]
        public async Task<ActionResult<List<Allergies>>> GetMyAllergies()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var allergies = await _allergyService.GetForUserAsync(userId);
            return Ok(allergies);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Allergies>> GetMyAllergy(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var allergy = await _allergyService.GetByIdForUserAsync(userId, id);
            if (allergy == null) return NotFound();
            return Ok(allergy);
        }
        [HttpPost]
        public async Task<ActionResult<Allergies>> CreateMyAllergy([FromBody] AllergyCreateUpdateDto
       dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var created = await _allergyService.CreateForUserAsync(userId, dto);
            return CreatedAtAction(nameof(GetMyAllergy), new { id = created.AllergyId }, created);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Allergies>> UpdateMyAllergy(int id, [FromBody]
AllergyCreateUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var updated = await _allergyService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMyAllergy(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            var success = await _allergyService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
