using System.ComponentModel.DataAnnotations;

namespace PersonalHealthRecordManagement.DTOs
{
    public class UpdateUserProfileDto
    {
        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150")]
        public int? Age { get; set; }

        [MaxLength(20, ErrorMessage = "Gender cannot exceed 20 characters")]
        public string? Gender { get; set; }

        [Range(0.1, 1000, ErrorMessage = "Weight must be between 0.1 and 1000 kg")]
        public decimal? Weight { get; set; }

        [MaxLength(10, ErrorMessage = "Blood group cannot exceed 10 characters")]
        public string? BloodGroup { get; set; }

        [MaxLength(20, ErrorMessage = "Emergency contact cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Emergencycontact { get; set; }
    }
}
