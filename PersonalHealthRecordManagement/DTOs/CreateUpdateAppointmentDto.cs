namespace PersonalHealthRecordManagement.DTOs
{
    public class CreateUpdateAppointmentDto
    {
        public String DoctorName { get; set; } = null!;
        public String Purpose { get; set; } = null!;

        public DateTime AppointmentDate { get; set; }

        public string? Status { get; set; }

    }
}
