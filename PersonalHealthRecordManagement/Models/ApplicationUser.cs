using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace PersonalHealthRecordManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public virtual UserProfile? UserProfile { get; set; }
    }
}
