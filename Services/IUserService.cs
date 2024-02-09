using DotNetAuthentication.Models;

namespace DotNetAuthentication.Services
{
    public interface IUserService
    {
        Task<User> AddUser(User user);
    }
}