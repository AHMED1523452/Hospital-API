namespace Hospital_API.Application.DTOs
{
    public class WaitingRoomDTO
    {
        public Guid? appoinmentId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public TimeOnly? startTime { get; set; }
        public string Status { get; set; }
        public bool IsPrepared { get; set; }
    }
}
