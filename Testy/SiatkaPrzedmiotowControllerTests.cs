using Microsoft.AspNetCore.Mvc;
using Moq;
using SANSylabusApi.DTOs;
using SylabusAPI.Controllers;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SylabusAPI.Tests
{
    public class SiatkaPrzedmiotowControllerTests
    {
        private readonly Mock<ISiatkaService> _svcMock;
        private readonly SiatkaPrzedmiotowController _controller;

        public SiatkaPrzedmiotowControllerTests()
        {
            _svcMock = new Mock<ISiatkaService>();
            _controller = new SiatkaPrzedmiotowController(_svcMock.Object);
        }

        [Fact]
        public async Task GetByPrzedmiot_ReturnsOkWithList()
        {
            var list = new List<SiatkaPrzedmiotowDto>
            {
                new SiatkaPrzedmiotowDto { Id = 1, PrzedmiotId = 10, Typ = "obowiazkowy", Wyklad = 30 }
            };

            _svcMock.Setup(s => s.GetByPrzedmiotAsync(10, "obowiazkowy"))
                    .ReturnsAsync(list);

            var result = await _controller.GetByPrzedmiot(10, "obowiazkowy");

            var ok = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<SiatkaPrzedmiotowDto>>(ok.Value);
            Assert.Single(items);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            _svcMock.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateSiatkaRequest>()))
                    .ReturnsAsync(true);

            var result = await _controller.Update(5, new UpdateSiatkaRequest());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdInvalid()
        {
            _svcMock.Setup(s => s.UpdateAsync(999, It.IsAny<UpdateSiatkaRequest>()))
                    .ReturnsAsync(false);

            var result = await _controller.Update(999, new UpdateSiatkaRequest());

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("999", notFound.Value!.ToString());
        }
    }
}
