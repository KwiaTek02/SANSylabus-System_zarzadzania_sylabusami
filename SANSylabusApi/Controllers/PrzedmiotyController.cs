using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    [Route("api/przedmioty")]
    public class PrzedmiotyController : ControllerBase
    {
        private readonly IPrzedmiotService _svc;
        public PrzedmiotyController(IPrzedmiotService svc) => _svc = svc;

        [HttpGet("kierunek/{kierunek}")]
        public async Task<IActionResult> GetByKierunek(string kierunek)
        {
            var list = await _svc.GetByKierunekAsync(kierunek);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }
    }
}