namespace Hospital_API.Application.DTOs
{
    public class ResponseDTOForGettingAPIs<T> where T : class
    {
        public T Data { get; set; }
    }
}
