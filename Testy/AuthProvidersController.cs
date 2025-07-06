using Xunit;
using Microsoft.AspNetCore.Mvc;
using SANSylabusApi.Controllers;
using SANSylabusApi.Settings;
using System.Text.Json;

namespace SylabusAPI.Tests
{
    public class AuthProvidersControllerTests
    {
        [Fact]
        public void Get_ReturnsOkWithCorrectGoogleStatus()
        {
            // Arrange
            var authStatus = new AuthProviderStatus { IsGoogleEnabled = true };
            var controller = new AuthProvidersController(authStatus);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var parsed = JsonSerializer.Deserialize<JsonElement>(json);
            Assert.True(parsed.GetProperty("google").GetBoolean());
        }

        [Fact]
        public void Get_ReturnsOkWithGoogleDisabled()
        {
            // Arrange
            var authStatus = new AuthProviderStatus { IsGoogleEnabled = false };
            var controller = new AuthProvidersController(authStatus);

            // Act
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var parsed = JsonSerializer.Deserialize<JsonElement>(json);
            Assert.False(parsed.GetProperty("google").GetBoolean());
        }
    }
}