namespace Hospital_API.Application.NurseDTO
{
    public class NurseWithAppoinmentDTO
    {
        public Guid? AppoinmentId { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public TimeOnly? StartAt { get; set; }
        public TimeOnly? EndAt { get; set; }
        public string Status { get; set; }
    }
}
