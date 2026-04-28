using FacturacionAPI.DTOs;

namespace FacturacionAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDto);
        Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto);
    }
}
