using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    // Ścieżka URL dla tego kontrolera
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        // Pole przechowujące referencję do serwisu autoryzacji
        private readonly IAuthService _auth;

        // Konstruktor przyjmujący implementację serwisu autoryzacji (wstrzykiwanie zależności)
        public AuthController(IAuthService auth) => _auth = auth;

        // Endpoint do rejestracji użytkownika
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _auth.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Obsługa błędu i zwrócenie odpowiedzi z kodem 400 (Bad Request)
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint do logowania użytkownika
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _auth.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                // Obsługa błędnych danych logowania i zwrócenie odpowiedzi z kodem 401 (Unauthorized)
                return Unauthorized(new { message = "Nieprawidłowy login lub hasło." });
            }
        }
    }
}
