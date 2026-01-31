namespace Hospital_API.Application.DTOs
{
    public class AuthResponseNewAccessTokenDTO
    {
        public string NewAccessToken { get; set; }
        public DateTime ExpireDateForNewAccessToken { get; set; }
    }
}
