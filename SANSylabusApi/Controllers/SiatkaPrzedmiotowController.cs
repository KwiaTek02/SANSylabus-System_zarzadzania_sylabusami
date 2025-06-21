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
        // GET: api/siatka/przedmiot/5/typ/obowiązkowy
        [HttpGet("przedmiot/{przedmiotId}/typ/{typ}")]
        public async Task<IActionResult> GetByPrzedmiot(int przedmiotId, string typ)
        {
            // Pobranie siatki powiązanej z konkretnym przedmiotem i typem zajęć
            var list = await _svc.GetByPrzedmiotAsync(przedmiotId, typ);

            // Zwrócenie danych w odpowiedzi HTTP 200 OK
            return Ok(list);
        }

        // Endpoint: Aktualizacja konkretnej siatki przedmiotów na podstawie ID
        // PUT: api/siatka/10
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSiatkaRequest req)
        {
            // Próba aktualizacji siatki
            var updated = await _svc.UpdateAsync(id, req);

            // Jeśli nie znaleziono rekordu do aktualizacji — HTTP 404 Not Found
            if (!updated)
                return NotFound($"Nie znaleziono siatki z id={id}");

            // Jeśli aktualizacja się powiodła — zwracamy HTTP 204 No Content
            return NoContent();
        }
    }
}
