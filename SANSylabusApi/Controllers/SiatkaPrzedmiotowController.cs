using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SANSylabusApi.DTOs;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    [Route("api/siatka")]
    public class SiatkaPrzedmiotowController : ControllerBase
    {
        private readonly ISiatkaService _svc;
        public SiatkaPrzedmiotowController(ISiatkaService svc) => _svc = svc;

        [HttpGet("przedmiot/{przedmiotId}/typ/{typ}")]
        public async Task<IActionResult> GetByPrzedmiot(int przedmiotId, string typ)
        {
            var list = await _svc.GetByPrzedmiotAsync(przedmiotId, typ);
            return Ok(list);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSiatkaRequest req)
        {
            var updated = await _svc.UpdateAsync(id, req);
            if (!updated)
                return NotFound($"Nie znaleziono siatki z id={id}");

            return NoContent();
        }
    }
}