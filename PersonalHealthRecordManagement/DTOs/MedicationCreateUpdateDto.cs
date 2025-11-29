namespace PersonalHealthRecordManagement.DTOs
{
    public class MedicationCreateUpdateDto
    {
        public string? MedicineName { get; set; }
        public int? Quantity { get; set; }
        public int? Frequency { get; set; }
        public string? PrescribedFor { get; set; }
        public string? PrescribedBy { get; set; }
        public DateTime? DatePrescribed { get; set; }

    }
}
