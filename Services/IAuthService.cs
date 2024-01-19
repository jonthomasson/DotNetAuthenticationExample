using DotNetAuthentication.Models;

namespace DotNetAuthentication.Services
{
    public interface IAuthService
    {
        string? GenerateToken(User user);
    }
}