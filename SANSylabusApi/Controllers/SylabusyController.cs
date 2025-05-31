using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    [Route("api/sylabusy")]
    public class SylabusyController : ControllerBase
    {
        private readonly ISylabusService _svc;

        //xd

        public SylabusyController(ISylabusService svc)
            => _svc = svc;

        [HttpGet("przedmiot/{przedmiotId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByPrzedmiot(int przedmiotId)
        {
            var list = await _svc.GetByPrzedmiotAsync(przedmiotId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // Tworzenie nowego sylabusu (tylko wykładowca lub admin)
        [Authorize(Roles = "wykladowca,admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SylabusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // Aktualizacja istniejącego sylabusu
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
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Usunięcie sylabusu
        [Authorize(Roles = "wykladowca,admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // 1) Sprawdź, czy w ogóle istnieje
            var existing = await _svc.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            try
            {
                // 2) Spróbuj usunąć
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                // 3) Jeżeli serwis rzuci, że nie jestes koordynatorem, zwróć 403
                return Forbid();
            }
        }

        [HttpGet("{sylabusId}/koordynator")]
        [AllowAnonymous]
        public async Task<IActionResult> GetKoordynator(int sylabusId)
        {
            var koordynator = await _svc.GetKoordynatorBySylabusIdAsync(sylabusId);
            if (koordynator == null)
                return NotFound();

            return Ok(koordynator);
        }

    }
}
