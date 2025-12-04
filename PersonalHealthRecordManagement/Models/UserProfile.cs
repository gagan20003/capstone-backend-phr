using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PersonalHealthRecordManagement.Models
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserProfileId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        public int? Age { get; set; }
        public string? Gender { get; set; }

        [Column(TypeName ="Number(4,2)")]
        public decimal? Weight { get; set; }

        public string? BloodGroup { get; set; }

        public string? Emergencycontact { get; set; }

        //define navigation properties here
        [JsonIgnore]
        public virtual ICollection<Allergies>? Allergies { get; set; } = new List<Allergies>();
        [JsonIgnore]
        public virtual ICollection<Medications>? Medications { get; set; } = new List<Medications>();

    }
}
