using System.ComponentModel.DataAnnotations;

namespace PersonalHealthRecordManagement.DTOs
{
    public class AllergyCreateUpdateDto
    {
        [Required(ErrorMessage = "Allergy name is required")]
        [MaxLength(100, ErrorMessage = "Allergy name cannot exceed 100 characters")]
        public string AllergyName { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Symptoms cannot exceed 500 characters")]
        public string? Symptoms { get; set; }

        [MaxLength(50, ErrorMessage = "Severity cannot exceed 50 characters")]
        public string? Severity { get; set; }
    }
}
