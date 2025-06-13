using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.Controllers;
using SylabusAPI.Services.Interfaces;
using SylabusAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SylabusAPI.Tests
{
    public class HistoriaControllerTests
    {
        private readonly Mock<IHistoriaService> _historiaServiceMock;
        private readonly HistoriaController _controller;

        public HistoriaControllerTests()
        {
            _historiaServiceMock = new Mock<IHistoriaService>();
            _controller = new HistoriaController(_historiaServiceMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithHistoriaList()
        {
            // Arrange
            var sylabusId = 5;

            var historiaList = new List<SylabusHistoriaDto>
            {
                new SylabusHistoriaDto
                {
                    Id = 1,
                    SylabusId = sylabusId,
                    CzasZmiany = DateTime.UtcNow,
                    DataZmiany = DateOnly.FromDateTime(DateTime.Today),
                    ZmienionyPrzez = 1,
                    OpisZmiany = "Dodano sekcję 1.2",
                    WersjaWtedy = "v1.0",
                    ZmieniajacyImieNazwiskoTytul = "dr Kowalski"
                }
            };

            _historiaServiceMock
                .Setup(s => s.GetBySylabusAsync(sylabusId))
                .ReturnsAsync(historiaList);

            // Act
            var result = await _controller.Get(sylabusId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<SylabusHistoriaDto>>(okResult.Value);
            Assert.Single(returnedList);
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenNoHistory()
        {
            // Arrange
            var sylabusId = 999;

            _historiaServiceMock
                .Setup(s => s.GetBySylabusAsync(sylabusId))
                .ReturnsAsync(new List<SylabusHistoriaDto>());

            // Act
            var result = await _controller.Get(sylabusId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<SylabusHistoriaDto>>(okResult.Value);
            Assert.Empty(list);
        }
    }
}
