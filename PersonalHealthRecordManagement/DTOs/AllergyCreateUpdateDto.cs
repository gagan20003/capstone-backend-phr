namespace PersonalHealthRecordManagement.DTOs
{
    public class AllergyCreateUpdateDto
    {
        public string? AllergyName { get; set; }
        public string? Symptoms { get; set; }
        public string? Severity
        {
            get; set;
        }
    }
}
