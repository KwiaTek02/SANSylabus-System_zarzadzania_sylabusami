using Microsoft.AspNetCore.Mvc;
using Moq;
using SylabusAPI.Controllers;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SylabusAPI.Tests
{
    public class PrzedmiotyControllerTests
    {
        private readonly Mock<IPrzedmiotService> _serviceMock;
        private readonly PrzedmiotyController _controller;

        public PrzedmiotyControllerTests()
        {
            _serviceMock = new Mock<IPrzedmiotService>();
            _controller = new PrzedmiotyController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetByKierunek_ReturnsOk_WithItems()
        {
            var kierunek = "Informatyka";
            var data = new List<PrzedmiotDto>
            {
                new PrzedmiotDto { Id = 1, Nazwa = "Programowanie" },
                new PrzedmiotDto { Id = 2, Nazwa = "Bazy danych" }
            };

            _serviceMock
                .Setup(s => s.GetByKierunekAsync(kierunek))
                .ReturnsAsync(data);

            var result = await _controller.GetByKierunek(kierunek);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<PrzedmiotDto>>(ok.Value);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenItemFound()
        {
            var przedmiot = new PrzedmiotDto { Id = 5, Nazwa = "Matematyka dyskretna" };

            _serviceMock
                .Setup(s => s.GetByIdAsync(5))
                .ReturnsAsync(przedmiot);

            var result = await _controller.Get(5);

            var ok = Assert.IsType<OkObjectResult>(result);
            var item = Assert.IsType<PrzedmiotDto>(ok.Value);
            Assert.Equal("Matematyka dyskretna", item.Nazwa);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenItemMissing()
        {
            _serviceMock
                .Setup(s => s.GetByIdAsync(404))
                .ReturnsAsync((PrzedmiotDto?)null);

            var result = await _controller.Get(404);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
