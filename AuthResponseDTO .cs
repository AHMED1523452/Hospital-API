namespace Hospital_API.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireDateForAccessToken { get; set; }
        public DateTime ExpireDateForRefreshToken { get; set; }
    }
}
