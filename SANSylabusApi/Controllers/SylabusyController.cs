using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    // Bazowy route: api/sylabusy
    [Route("api/sylabusy")]
    public class SylabusyController : ControllerBase
    {
        // Serwis obsługujący operacje na sylabusach
        private readonly ISylabusService _svc;

        // Konstruktor z wstrzyknięciem serwisu ISylabusService
        public SylabusyController(ISylabusService svc)
            => _svc = svc;

        // Pobiera wszystkie sylabusy powiązane z danym przedmiotem
        // Dostęp anonimowy
        [HttpGet("przedmiot/{przedmiotId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByPrzedmiot(int przedmiotId)
        {
            var list = await _svc.GetByPrzedmiotAsync(przedmiotId);
            return Ok(list);
        }

        // Pobiera konkretny sylabus po ID
        // Dostęp anonimowy
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // Tworzenie nowego sylabusa
        // Tylko dla użytkowników z rolą "wykladowca" lub "admin"
        [Authorize(Roles = "wykladowca,admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SylabusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _svc.CreateAsync(dto);

            // Zwraca HTTP 201 Created z lokalizacją nowego zasobu
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // Aktualizacja istniejącego sylabusa
        // Dostęp tylko dla wykładowcy lub admina
        [Authorize(Roles = "wykladowca,admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSylabusRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _svc.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            try
            {
                await _svc.UpdateAsync(id, req);
                return NoContent(); // Zwraca 204 po udanej aktualizacji
            }
            catch (UnauthorizedAccessException)
            {
                // Użytkownik nie ma uprawnień do edycji tego sylabusa
                return Forbid();
            }
        }

        // Usunięcie sylabusa
        // Tylko dla wykładowcy lub admina
        [Authorize(Roles = "wykladowca,admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Sprawdzenie czy sylabus istnieje
            var existing = await _svc.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            try
            {
                // Próba usunięcia sylabusa
                await _svc.DeleteAsync(id);
                return NoContent(); // Zwraca 204 jeśli sukces
            }
            catch (UnauthorizedAccessException)
            {
                // Brak odpowiednich uprawnień (np. nie jesteś koordynatorem)
                return Forbid();
            }
        }

        // Pobiera dane koordynatora przypisanego do danego sylabusa
        [HttpGet("{sylabusId}/koordynator")]
        [AllowAnonymous]
        public async Task<IActionResult> GetKoordynator(int sylabusId)
        {
            var koordynator = await _svc.GetKoordynatorBySylabusIdAsync(sylabusId);
            if (koordynator == null)
                return NotFound();

            return Ok(koordynator);
        }

        // Sprawdza, czy dany użytkownik jest koordynatorem sylabusa
        [HttpGet("{sylabusId}/czy-koordynator/{userId}")]
        [AllowAnonymous] // Można dodać [Authorize] jeśli wymagamy tokenu
        public async Task<IActionResult> CzyKoordynator(int sylabusId, int userId)
        {
            var isKoordynator = await _svc.IsKoordynatorAsync(sylabusId, userId);
            return Ok(isKoordynator);
        }
    }
}
