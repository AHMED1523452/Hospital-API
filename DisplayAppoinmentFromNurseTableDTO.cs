namespace Hospital_API.Application.NurseDTO
{
    public class DisplayAppoinmentFromNurseTableDTO
    {
        public Guid? appoinmentId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateOnly? AppoinmentDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string reason { get; set; }
        public string Status { get; set; }
        public bool IsPrepared { get; set; }
    }
}
