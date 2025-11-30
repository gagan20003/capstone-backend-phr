using Microsoft.AspNetCore.Mvc;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : BaseController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        /// <summary>
        /// Schedule a new appointment
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Appointments>> CreateAppointment([FromBody] CreateUpdateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Appointments>();

            // Business rule: Appointment date should not be in the past
            if (dto.AppointmentDate < DateTime.UtcNow)
            {
                return BadRequestResponse<Appointments>("Appointment date cannot be in the past");
            }

            try
            {
                var created = await _appointmentService.CreateForUserAsync(userId, dto);
                _logger.LogInformation("Appointment created: AppointmentId={AppointmentId}, UserId={UserId}", created.AppointmentId, userId);
                return CreatedAtAction(nameof(GetAppointment), new { id = created.AppointmentId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment for UserId={UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Get all appointments for logged-in user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Appointments>>> GetAppointments()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<List<Appointments>>();

            var appointments = await _appointmentService.GetForUserAsync(userId);
            return Ok(appointments);
        }

        /// <summary>
        /// Get details of a specific appointment
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Appointments>> GetAppointment(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Appointments>();

            var appointment = await _appointmentService.GetByIdForUserAsync(userId, id);
            if (appointment == null) return NotFoundResponse<Appointments>("Appointment not found");

            return Ok(appointment);
        }

        /// <summary>
        /// Update or reschedule an appointment
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Appointments>> UpdateAppointment(int id, [FromBody] CreateUpdateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetCurrentUserId();
            if (userId == null) return UnauthorizedResponse<Appointments>();

            // Business rule: Appointment date should not be in the past
            if (dto.AppointmentDate < DateTime.UtcNow)
            {
                return BadRequestResponse<Appointments>("Appointment date cannot be in the past");
            }

            var updated = await _appointmentService.UpdateForUserAsync(userId, id, dto);
            if (updated == null) return NotFoundResponse<Appointments>("Appointment not found");

            _logger.LogInformation("Appointment updated: AppointmentId={AppointmentId}, UserId={UserId}", id, userId);
            return Ok(updated);
        }

        /// <summary>
        /// Cancel an appointment
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var success = await _appointmentService.DeleteForUserAsync(userId, id);
            if (!success) return NotFound();

            _logger.LogInformation("Appointment deleted: AppointmentId={AppointmentId}, UserId={UserId}", id, userId);
            return NoContent();
        }
    }
}