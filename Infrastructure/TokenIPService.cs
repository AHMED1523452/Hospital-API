namespace Hospital_API.Services
{
    public class TokenIPService : ITokenIPService
    {
        private readonly IHttpContextAccessor httpContext;

        public TokenIPService(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
        }

        public string GetClientIP()
        {
            return httpContext.HttpContext?.Connection.RemoteIpAddress.ToString();
        }
    }
}
