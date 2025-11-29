namespace PersonalHealthRecordManagement.DTOs
{
    public class CreateUpdateMedicalRecordDto
    {
        public string RecordType { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string? Description { get; set; } 
        public DateTime RecordDate { get; set; }
        public string FileUrl { get; set; } = null!;

    }
}
