namespace Hospital_API.Domain.Model
{
    public class Presception
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        public Guid DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        public Guid? AppoinmentId { get; set; }
        public Appoinment? Appoinment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public List<PresceptionMedicine>? presceptionMedicines { get; set; }
    }
}
