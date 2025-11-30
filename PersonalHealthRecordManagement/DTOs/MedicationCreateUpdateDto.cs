using System.ComponentModel.DataAnnotations;

namespace PersonalHealthRecordManagement.DTOs
{
    public class MedicationCreateUpdateDto
    {
        [Required(ErrorMessage = "Medicine name is required")]
        [MaxLength(100, ErrorMessage = "Medicine name cannot exceed 100 characters")]
        public string MedicineName { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int? Quantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Frequency must be greater than 0")]
        public int? Frequency { get; set; }

        [MaxLength(200, ErrorMessage = "Prescribed for cannot exceed 200 characters")]
        public string? PrescribedFor { get; set; }

        [MaxLength(100, ErrorMessage = "Prescribed by cannot exceed 100 characters")]
        public string? PrescribedBy { get; set; }

        public DateTime? DatePrescribed { get; set; }
    }
}
