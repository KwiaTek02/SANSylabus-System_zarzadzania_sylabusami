using Microsoft.AspNetCore.Mvc;
using SylabusAPI.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SylabusAPI.Controllers
{
    [ApiController]
    [Route("api/uzytkownicy")]
    public class UserController : ControllerBase
    {
        private readonly SyllabusContext _db;

        public UserController(SyllabusContext db)
        {
            _db = db;
        }

        [HttpGet("nazwisko")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetNameByLogin([FromQuery] string login)
        {
            try
            {
                var user = await _db.uzytkownicies
                    .Where(u => u.login == login)
                    .Select(u => new
                    {
                        PelneImie = (u.tytul != null ? u.tytul + " " : "") + u.imie_nazwisko
                    })
                    .FirstOrDefaultAsync();

                return user is not null
                    ? Ok(user.PelneImie)
                    : NotFound("Nie znaleziono użytkownika.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Błąd w GetNameByLogin: {ex.Message}");
                return StatusCode(500, "Wystąpił błąd podczas pobierania danych użytkownika.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _db.uzytkownicies
                    .Where(u => u.id == id)
                    .Select(u => new
                    {
                        Id = u.id,
                        ImieNazwisko = u.imie_nazwisko,
                        Tytul = u.tytul
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound("Nie znaleziono użytkownika o podanym ID.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Błąd w GetById: {ex.Message}");
                return StatusCode(500, "Wystąpił błąd podczas pobierania danych użytkownika.");
            }
        }

    }
}