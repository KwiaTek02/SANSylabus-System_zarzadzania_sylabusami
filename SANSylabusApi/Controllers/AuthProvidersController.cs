using Microsoft.AspNetCore.Mvc;
using SANSylabusApi.Settings;

namespace SANSylabusApi.Controllers
{
    [ApiController]
    [Route("api/auth/providers")]
    public class AuthProvidersController: ControllerBase
    {
        private readonly AuthProviderStatus _authStatus;

        public AuthProvidersController(AuthProviderStatus authStatus)
        {
            _authStatus = authStatus;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { google = _authStatus.IsGoogleEnabled });
        }
    }
}
