using System.ComponentModel.DataAnnotations;

namespace PersonalHealthRecordManagement.DTOs
{
    public class CreateUpdateMedicalRecordDto
    {
        [Required(ErrorMessage = "Record type is required")]
        [MaxLength(50, ErrorMessage = "Record type cannot exceed 50 characters")]
        public string RecordType { get; set; } = null!;

        [Required(ErrorMessage = "Provider is required")]
        [MaxLength(100, ErrorMessage = "Provider cannot exceed 100 characters")]
        public string Provider { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Record date is required")]
        public DateTime RecordDate { get; set; }

        [Required(ErrorMessage = "File URL is required")]
        [MaxLength(200, ErrorMessage = "File URL cannot exceed 200 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string FileUrl { get; set; } = null!;
    }
}
