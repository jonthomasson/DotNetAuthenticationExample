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
        public async Task<IActionResult> Authenticate(string username, string password)
        {
            //var user = new User() { Email = "jthomasson@sjcoe.net", Name = "Jon", UserName = "jthomasson", Id = 1234, Role = Role.Admin };
            //var user = await _authService.CheckLoginSimple(username, password);
            var user = await _authService.CheckLoginHash(username, password);


            if (user == null)
            {
                return Unauthorized();
            }

            var token = _authService.GenerateToken(user);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}
