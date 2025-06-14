using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.Controllers;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SylabusAPI.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object); // ✅ logger przekazany
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            // Arrange
            var request = new RegisterRequest
            {
                ImieNazwisko = "Jan Kowalski",
                Login = "jkowalski",
                Password = "Haslo1",
                Email = "jk@wp.pl",
                TypKonta = "gosc"
            };

            _authServiceMock
                .Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(new AuthResponse { Token = "mock_token", ExpiresAt = DateTime.UtcNow.AddHours(1) });

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal("mock_token", response.Token);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenLoginIsTaken()
        {
            var request = new RegisterRequest
            {
                ImieNazwisko = "Anna",
                Login = "istnieje",
                Password = "Haslo1",
                Email = "anna@example.com",
                TypKonta = "gosc"
            };

            _authServiceMock
                .Setup(s => s.RegisterAsync(request))
                .ThrowsAsync(new Exception("Ten login jest już zajęty."));

            var result = await _controller.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("login", badRequest.Value!.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenInvalidPassword()
        {
            var request = new RegisterRequest
            {
                ImieNazwisko = "Zły",
                Login = "zly",
                Password = "niewystarczajace",
                Email = "zly@example.com",
                TypKonta = "gosc"
            };

            _authServiceMock
                .Setup(s => s.RegisterAsync(request))
                .ThrowsAsync(new Exception("Hasło musi zawierać co najmniej jedną dużą literę i jedną cyfrę."));

            var result = await _controller.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Hasło", badRequest.Value!.ToString());
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            var request = new LoginRequest
            {
                Login = "login",
                Password = "Haslo1"
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(new AuthResponse { Token = "valid_token", ExpiresAt = DateTime.UtcNow.AddHours(1) });

            var result = await _controller.Login(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(ok.Value);
            Assert.Equal("valid_token", response.Token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            var request = new LoginRequest
            {
                Login = "invalid",
                Password = "wrong"
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
