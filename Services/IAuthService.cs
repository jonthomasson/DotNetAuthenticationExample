using DotNetAuthentication.Models;

namespace DotNetAuthentication.Services
{
    public interface IAuthService
    {
        string? GenerateToken(User user);
        //Task<User?> CheckLoginSimple(string username, string password);
        Task<User?> CheckLoginHash(string username, string password);
        Task SetPassword(User user, string newPassword);
    }
}