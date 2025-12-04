using System.ComponentModel.DataAnnotations;

namespace PersonalHealthRecordManagement.DTOs
{
    public class CreateUpdateAppointmentDto
    {
        [Required(ErrorMessage = "Doctor name is required")]
        [MaxLength(100, ErrorMessage = "Doctor name cannot exceed 100 characters")]
        public string DoctorName { get; set; } = null!;

        [Required(ErrorMessage = "Purpose is required")]
        [MaxLength(100, ErrorMessage = "Purpose cannot exceed 100 characters")]
        public string Purpose { get; set; } = null!;

        [Required(ErrorMessage = "Appointment date is required")]
        public DateTime AppointmentDate { get; set; }

        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }
    }
}
