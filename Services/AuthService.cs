using DotNetAuthentication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNetAuthentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly DotNetAuthenticationDbContext _context;
        public AuthService(IConfiguration configuration, DotNetAuthenticationDbContext context)
        {
            _configuration = configuration;
            _context = context;

        }

        /// <summary>
        /// simple authentication, but not a good approach since it stores the password in plain text. See CheckLoginHash instead.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //public async Task<User?> CheckLoginSimple(string username, string password)
        //{
        //    var user = await _context.Users.Where(u => u.UserName == username && u.Password == password).FirstOrDefaultAsync();

        //    if (user == null) return null;

        //    return user;
        //}

        public async Task<User?> CheckLoginHash(string username, string password)
        {
            // Retrieve user from the database
            var user = await _context.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
            if (user == null) return null;

            // Verify the hashed password
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i]) return null;
                }
            }

            return user;
        }

        public async Task SetPassword(User user, string newPassword)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Password cannot be empty or whitespace only string.", nameof(newPassword));

            using (var hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
            }

            // Update the user in the database with the new password hash and salt
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public string? GenerateToken(User user)
        {
            if (user == null) return null;
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration.GetSection("JwtSettings:Issuer").Value!,
                Audience = _configuration.GetSection("JwtSettings:Audience").Value!,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
