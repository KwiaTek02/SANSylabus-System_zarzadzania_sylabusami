using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SylabusAPI.Controllers;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace SylabusAPI.Tests
{
    public class ExternalAuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly ExternalAuthController _controller;

        public ExternalAuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new ExternalAuthController(_authServiceMock.Object);
        }

        [Fact]
        public void GoogleLogin_ReturnsChallengeResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("/auth/externalauth/google-response?returnUrl=%2Fcallback");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _controller.Url = urlHelperMock.Object;

            // Act
            var result = _controller.GoogleLogin("/callback");

            // Assert
            var challengeResult = Assert.IsType<ChallengeResult>(result);
            Assert.Equal("Google", challengeResult.AuthenticationSchemes[0]);
            Assert.NotNull(challengeResult.Properties?.RedirectUri);
            Assert.Contains("google-response", challengeResult.Properties.RedirectUri);
        }

        [Fact]
        public async Task GoogleResponse_ReturnsUnauthorized_WhenAuthFails()
        {
            // Arrange
            var authResult = AuthenticateResult.Fail("fail");
            var mockAuth = new Mock<IAuthenticationService>();
            var context = new DefaultHttpContext();
            context.RequestServices = new ServiceCollection()
                .AddSingleton<IAuthenticationService>(mockAuth.Object)
                .BuildServiceProvider();

            mockAuth
                .Setup(a => a.AuthenticateAsync(context, "Cookies"))
                .ReturnsAsync(authResult);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            // Act
            var result = await _controller.GoogleResponse();

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("Logowanie Google", unauthorized.Value!.ToString());
        }

        [Fact]
        public async Task GoogleResponse_ReturnsTokenScriptHtml_WhenAuthSucceeds()
        {
            // Arrange: Claims + Identity + Principal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Name, "Tester")
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            var authResult = AuthenticateResult.Success(new AuthenticationTicket(principal, "Cookies"));

            var mockAuth = new Mock<IAuthenticationService>();
            var context = new DefaultHttpContext();
            context.RequestServices = new ServiceCollection()
                .AddSingleton<IAuthenticationService>(mockAuth.Object)
                .BuildServiceProvider();

            mockAuth
                .Setup(a => a.AuthenticateAsync(context, "Cookies"))
                .ReturnsAsync(authResult);

            _authServiceMock
                .Setup(a => a.GoogleLoginAsync("test@example.com", "Tester"))
                .ReturnsAsync(new AuthResponse { Token = "mock-jwt-token" });

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            // Act
            var result = await _controller.GoogleResponse();

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("text/html", contentResult.ContentType);
            Assert.Contains("mock-jwt-token", contentResult.Content!);
            Assert.Contains("window.opener.postMessage", contentResult.Content!);
        }

        [Fact]
        public async Task GoogleResponse_ReturnsUnauthorized_WhenEmailMissing()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Tester") };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            var authResult = AuthenticateResult.Success(new AuthenticationTicket(principal, "Cookies"));

            var mockAuth = new Mock<IAuthenticationService>();
            var context = new DefaultHttpContext();
            context.RequestServices = new ServiceCollection()
                .AddSingleton<IAuthenticationService>(mockAuth.Object)
                .BuildServiceProvider();

            mockAuth
                .Setup(a => a.AuthenticateAsync(context, "Cookies"))
                .ReturnsAsync(authResult);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            // Act
            var result = await _controller.GoogleResponse();

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("Brak adresu email", unauthorized.Value!.ToString());
        }
    }
}
