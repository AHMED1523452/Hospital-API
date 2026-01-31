using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Hospital_API.Application.DTOs
{
    public class BadRequesDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
