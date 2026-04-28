using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FacturacionAPI.Data;
using FacturacionAPI.DTOs;
using FacturacionAPI.Entities;

namespace FacturacionAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        
        public AuthService(ApplicationDbContext context, IConfiguration configuration) 
        { 
            _context = context; 
            _configuration = configuration; 
        }
        
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
        
        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email)) 
                return null;
                
            var user = new User 
            { 
                Username = registerDto.Username, 
                Email = registerDto.Email, 
                PasswordHash = HashPassword(registerDto.Password), 
                Role = "Usuario" 
            };
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return new AuthResponseDTO 
            { 
                Token = GenerateJwtToken(user), 
                Email = user.Email, 
                Role = user.Role 
            };
        }
        
        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null) return null;
            
            if (!VerifyPassword(loginDto.Password, user.PasswordHash)) return null;
                
            return new AuthResponseDTO 
            { 
                Token = GenerateJwtToken(user), 
                Email = user.Email, 
                Role = user.Role 
            };
        }
        
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("MiClaveSecreta12345678901234567890");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                    new Claim(ClaimTypes.Email, user.Email), 
                    new Claim(ClaimTypes.Role, user.Role ?? "Usuario") 
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
