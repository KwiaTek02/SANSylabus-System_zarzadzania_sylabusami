using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SANSylabusApi.DTOs;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    // Ustalenie bazowego route'u: api/siatka
    [Route("api/siatka")]
    public class SiatkaPrzedmiotowController : ControllerBase
    {
        // Serwis odpowiedzialny za operacje na siatkach przedmiotów
        private readonly ISiatkaService _svc;

        // Konstruktor z wstrzyknięciem serwisu ISiatkaService
        public SiatkaPrzedmiotowController(ISiatkaService svc) => _svc = svc;

        // Endpoint: Pobranie danych siatki dla danego przedmiotu i typu (np. "obowiązkowy", "fakultatywny")
        [HttpGet("przedmiot/{przedmiotId}/typ/{typ}")]
        public async Task<IActionResult> GetByPrzedmiot(int przedmiotId, string typ)
        {
            // Pobranie siatki powiązanej z konkretnym przedmiotem i typem zajęć
            var list = await _svc.GetByPrzedmiotAsync(przedmiotId, typ);

            return Ok(list);
        }

        // Endpoint: Aktualizacja konkretnej siatki przedmiotów na podstawie ID
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
