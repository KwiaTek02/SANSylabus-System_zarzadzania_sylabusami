using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.Services.Interfaces;
using SylabusAPI.DTOs;
using System.Security.Claims;

namespace SylabusAPI.Controllers
{
    [AllowAnonymous]
    [Route("auth/[controller]")]
    public class ExternalAuthController : Controller
    {
        private readonly IAuthService _authService;

        public ExternalAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("signin-google")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl })
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync("Cookies");

            if (!result.Succeeded)
                return Unauthorized("Logowanie Google się nie powiodło.");

            var claims = result.Principal!.Identities
                .FirstOrDefault()?
                .Claims
                .ToDictionary(c => c.Type, c => c.Value);

            var email = claims?.GetValueOrDefault(ClaimTypes.Email);
            var name = claims?.GetValueOrDefault(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Brak adresu email z Google");

            // Logika: Jeśli użytkownik nie istnieje → załóż konto, inaczej zaloguj
            var tokenResponse = await _authService.GoogleLoginAsync(email, name ?? "Użytkownik");

            var html = $@"
<html>
  <body>
    <script>
      window.opener.postMessage({{ token: '{tokenResponse.Token}' }}, 'https://localhost:7033');
      window.close();
    </script>
    <p>Logowanie zakończone. Możesz zamknąć to okno.</p>
  </body>
</html>";
            return Content(html, "text/html");
        }
    }
}
