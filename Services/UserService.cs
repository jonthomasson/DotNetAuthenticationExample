using DotNetAuthentication.Models;

namespace DotNetAuthentication.Services;

public class UserService : IUserService
{
    private readonly DotNetAuthenticationDbContext _context;
    private readonly IAuthService _authService;
    public UserService(DotNetAuthenticationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }
    public async Task<User> AddUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
