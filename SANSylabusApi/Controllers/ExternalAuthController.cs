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
    // Ustala ścieżkę: auth/externalauth (bo [controller] to ExternalAuth)
    [Route("auth/[controller]")]
    public class ExternalAuthController : Controller
    {
        // Serwis odpowiedzialny za logikę autoryzacji
        private readonly IAuthService _authService;

        // Wstrzyknięcie serwisu autoryzacji przez konstruktor
        public ExternalAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Endpoint inicjujący logowanie Google (przekierowuje użytkownika do Google)
        [HttpGet("signin-google")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            // Ustawienia przekierowania po zalogowaniu z Google
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl })
            };

            // Rozpoczęcie procesu uwierzytelniania przez Google
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        // Endpoint odbierający odpowiedź od Google po uwierzytelnieniu
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            // Próba uwierzytelnienia użytkownika na podstawie cookies
            var result = await HttpContext.AuthenticateAsync("Cookies");

            // Jeśli uwierzytelnienie się nie powiodło
            if (!result.Succeeded)
                return Unauthorized("Logowanie Google się nie powiodło.");

            // Pobranie roszczeń (claims) użytkownika z odpowiedzi Google
            var claims = result.Principal!.Identities
                .FirstOrDefault()?
                .Claims
                .ToDictionary(c => c.Type, c => c.Value);

            // Wyciągnięcie emaila i imienia z roszczeń
            var email = claims?.GetValueOrDefault(ClaimTypes.Email);
            var name = claims?.GetValueOrDefault(ClaimTypes.Name);

            // Jeśli email nie został zwrócony – zakończ operację błędem
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Brak adresu email z Google");

            // Jeśli użytkownik nie istnieje – utwórz go, jeśli istnieje – zaloguj
            var tokenResponse = await _authService.GoogleLoginAsync(email, name ?? "Użytkownik");

            // Skrypt JavaScript przekazuje token JWT do aplikacji głównej (np. SPA) i zamyka popup
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

            // Zwrócenie dynamicznie wygenerowanej strony HTML z tokenem
            return Content(html, "text/html");
        }
    }
}
