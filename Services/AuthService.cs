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
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
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

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            passwordSalt = RandomNumberGenerator.GetBytes(keySize);
            passwordHash = Rfc2898DeriveBytes.Pbkdf2(
       Encoding.UTF8.GetBytes(password),
       passwordSalt,
       iterations,
       hashAlgorithm,
       keySize);
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, storedSalt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, storedHash);
        }

        public async Task<User?> CheckLoginHash(string username, string password)
        {
            // Retrieve user from the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return null;

            // Verify the hashed password using PBKDF2
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }


        public async Task SetPassword(User user, string newPassword)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Password cannot be empty or whitespace only string.", nameof(newPassword));

            // Create password hash and salt using PBKDF2
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

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
