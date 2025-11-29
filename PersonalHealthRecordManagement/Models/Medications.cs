using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalHealthRecordManagement.Models
{
    public class Medications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicationId { get; set; }

        // FK to UserProfile
        [Required]
        public int UserProfileId { get; set; }

        [JsonIgnore]

        [ForeignKey(nameof(UserProfileId))]
        public virtual UserProfile UserProfile { get; set; } = null!;

        public string? MedicineName { get; set; }
        public int? Quantity { get; set; }
        public int? Frequency { get; set; }
        public string? PrescribedFor { get; set; }
        public string? PrescribedBy { get; set; }
        public DateOnly? DatePrescribed { get; set; }
    }
}