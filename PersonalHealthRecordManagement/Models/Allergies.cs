using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalHealthRecordManagement.Models
{
    public class Allergies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllergyId { get; set; }

        // FK to UserProfile
        [Required]
        public int UserProfileId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserProfileId))]
        public virtual UserProfile UserProfile { get; set; } = null!;

        public string? AllergyName { get; set; }
        public string? Symptoms { get; set; }
        public string? Severity { get; set; }
    }
}