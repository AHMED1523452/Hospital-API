using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Hospital_API.Application.PrescptionsDTO
{
    public class UpdatePresceptionDTO
    {
        public string Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
