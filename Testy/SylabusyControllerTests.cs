using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.Controllers;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using SANSylabusApi.DTOs;

namespace SylabusAPI.Tests
{
    public class SylabusyControllerTests
    {
        private readonly Mock<ISylabusService> _serviceMock;
        private readonly SylabusyController _controller;

        public SylabusyControllerTests()
        {
            _serviceMock = new Mock<ISylabusService>();
            _controller = new SylabusyController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetByPrzedmiot_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetByPrzedmiotAsync(1)).ReturnsAsync(new List<SylabusDto> {
                new SylabusDto { Id = 1, PrzedmiotId = 1 }
            });

            var result = await _controller.GetByPrzedmiot(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<SylabusDto>>(ok.Value);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNotExists()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((SylabusDto?)null);
            var result = await _controller.Get(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetKoordynator_ReturnsNotFound_IfNull()
        {
            _serviceMock.Setup(s => s.GetKoordynatorBySylabusIdAsync(999)).ReturnsAsync((KoordynatorDto?)null);
            var result = await _controller.GetKoordynator(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CzyKoordynator_ReturnsTrue()
        {
            _serviceMock.Setup(s => s.IsKoordynatorAsync(5, 1)).ReturnsAsync(true);
            var result = await _controller.CzyKoordynator(5, 1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)ok.Value!);
        }
    }
}
