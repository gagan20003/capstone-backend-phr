using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalHealthRecordManagement.Models
{
    public class Appointments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [MaxLength(100)]
        public string DoctorName { get; set; } = null!;
        [MaxLength(100)]
        public string Purpose { get; set; } = null!;

        public DateTime AppointmentDate { get; set; }

        [MaxLength(20)]
        [Column("STATUS")]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
