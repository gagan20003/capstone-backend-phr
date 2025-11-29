namespace PersonalHealthRecordManagement.DTOs
{
    public class UpdateUserProfileDto
    {
        public int? Age { get; set; }
        public string? Gender { get; set; }

        public Decimal? Weight { get; set; }

        public string? BloodGroup { get; set; }

        public string? Emergencycontact { get; set; }

        //can also add later for allergies and medications as well
    }
}
