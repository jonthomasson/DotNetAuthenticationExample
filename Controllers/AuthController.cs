using DotNetAuthentication.Models;
using DotNetAuthentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAuthentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost(Name = "authenticate")]
        public IActionResult Authenticate()
        {
            var user = new User() { Email = "jthomasson@sjcoe.net", Name = "Jon", UserName = "jthomasson", Id = 1234, Role = Role.Admin };
            var token = _authService.GenerateToken(user);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}
