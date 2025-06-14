using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _auth;
        

        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {

            _logger.LogInformation("Rejestracja użytkownika {Email}", request.Email);

            try
            {
                var response = await _auth.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd rejestracji użytkownika {Email}", request.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            _logger.LogInformation("Użytkownik {Login} próbuje się zalogować", request.Login);

            try
            {
                var response = await _auth.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Nieudana próba logowania: {Login}", request.Login);
                return Unauthorized(new { message = "Nieprawidłowy login lub hasło." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas logowania użytkownika {Login}", request.Login);
                return StatusCode(500, new { message = "Wewnętrzny błąd serwera." });
            }
        }
    }
}