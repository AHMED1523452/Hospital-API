using Microsoft.AspNetCore.SignalR;

namespace Hospital_API.Domain.Model
{
    public class RefreshTokens
    {
        public Guid Id { get; set; }
        public string UserID { get; set; }
        public string RefreshToken { get; set; }
        public string CreatedByIp { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime ExpireDateForRefreshToken { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
