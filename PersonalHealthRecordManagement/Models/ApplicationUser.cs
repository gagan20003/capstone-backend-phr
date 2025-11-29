using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace PersonalHealthRecordManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set;  }

        

        public virtual UserProfile? UserProfile { get; set; }

        [JsonIgnore]
        public virtual ICollection<Appointments> Appointments { get; set; } = new List<Appointments>();
        [JsonIgnore]
        public virtual ICollection<MedicalRecords> MedicalRecords { get; set; } = new List<MedicalRecords>();
        
    }
}
