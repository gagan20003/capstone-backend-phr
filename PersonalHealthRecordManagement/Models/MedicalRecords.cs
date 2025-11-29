using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PersonalHealthRecordManagement.Models
{
    public class MedicalRecords
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User {get; set;} = null!;

        [MaxLength(50)]
        public string RecordType { get; set; } = null!;

        [MaxLength(100)]
        public string Provider { get; set; } = null!;

        [MaxLength(200)]
        public string? Description { get; set; }

        public DateOnly RecordDate { get; set; }

        [MaxLength(200)]
        public string FileUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
