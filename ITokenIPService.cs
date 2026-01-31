using Microsoft.Identity.Client;

namespace Hospital_API.Services
{
    public interface ITokenIPService
    {
        string GetClientIP();
    }
}
