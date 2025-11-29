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
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // POST /api/appointments
        // Schedule a new appointment
        [HttpPost]
        public async Task<ActionResult<Appointments>> CreateAppointment(
            [FromBody] CreateUpdateAppointmentDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var created = await _appointmentService.CreateForUserAsync(userId, dto);
            return CreatedAtAction(nameof(GetAppointment), new { id = created.AppointmentId }, created);
        }

        // GET /api/appointments
        // Get all appointments for logged-in user
        [HttpGet]
        public async Task<ActionResult<List<Appointments>>> GetAppointments()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var appointments = await _appointmentService.GetForUserAsync(userId);
            return Ok(appointments);
        }

        // GET /api/appointments/{id}
        // Get details of a specific appointment
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Appointments>> GetAppointment(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var appointment = await _appointmentService.GetByIdForUserAsync(userId, id);
            if (appointment == null) return NotFound();

            return Ok(appointment);
        }

        // PUT /api/appointments/{id}
        // Update or reschedule an appointment
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Appointments>> UpdateAppointment(
            int id,
            [FromBody] CreateUpdateAppointmentDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updated = await _appointmentService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE /api/appointments/{id}
        // Cancel an appointment
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _appointmentService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}