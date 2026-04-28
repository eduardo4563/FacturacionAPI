using Microsoft.AspNetCore.Mvc;
using FacturacionAPI.DTOs;
using FacturacionAPI.Services;

namespace FacturacionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) { _authService = authService; }
        
        [HttpPost("register")] 
        public async Task<IActionResult> Register(RegisterDTO dto) { var result = await _authService.RegisterAsync(dto); if (result == null) return BadRequest(new { message = "Email ya registrado" }); return Ok(result); }
        
        [HttpPost("login")] 
        public async Task<IActionResult> Login(LoginDTO dto) { var result = await _authService.LoginAsync(dto); if (result == null) return Unauthorized(new { message = "Credenciales incorrectas" }); return Ok(result); }
    }
}
