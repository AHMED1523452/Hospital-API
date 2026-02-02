using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Domain.Model
{
    public class Nurse
    {
        [Key]
        public Guid? Id { get; set; }
        public string Department { get; set; }
        // FK
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<vitalSigns> vitalSigns { get; set; }
       
    }
}
