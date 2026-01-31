using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.DTOs
{
    public class NewResponseDTO<T> 
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
