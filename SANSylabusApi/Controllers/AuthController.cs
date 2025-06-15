using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

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
                return BadRequest(new { message = ex.Message });
            }
        }

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
                return Unauthorized(new { message = "Nieprawidłowy login lub hasło." });
            }
        }
    }
}