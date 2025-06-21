using Microsoft.AspNetCore.Mvc;
using SylabusAPI.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SylabusAPI.Controllers
{
    [ApiController]
    // Trasa bazowa dla tego kontrolera: api/uzytkownicy
    [Route("api/uzytkownicy")]
    public class UserController : ControllerBase
    {
        // Kontekst bazy danych (Entity Framework)
        private readonly SyllabusContext _db;

        // Konstruktor z wstrzyknięciem zależności (DbContext)
        public UserController(SyllabusContext db)
        {
            _db = db;
        }

        // Endpoint GET api/uzytkownicy/nazwisko?login=login
        // Zwraca pełne imię i nazwisko (z tytułem) użytkownika na podstawie loginu
        [HttpGet("nazwisko")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> GetNameByLogin([FromQuery] string login)
        {
            try
            {
                var user = await _db.uzytkownicies
                    .Where(u => u.login == login)
                    .Select(u => new
                    {
                        // Tworzenie pełnego imienia: jeśli tytuł istnieje → dodaj przed imieniem
                        PelneImie = (u.tytul != null ? u.tytul + " " : "") + u.imie_nazwisko
                    })
                    .FirstOrDefaultAsync();

                // Jeśli użytkownik został znaleziony, zwróć jego pełne imię
                return user is not null
                    ? Ok(user.PelneImie)
                    : NotFound("Nie znaleziono użytkownika.");
            }
            catch (Exception ex)
            {
                // Logowanie błędu do konsoli i zwrócenie kodu 500
                Console.Error.WriteLine($"❌ Błąd w GetNameByLogin: {ex.Message}");
                return StatusCode(500, "Wystąpił błąd podczas pobierania danych użytkownika.");
            }
        }

        // Endpoint GET api/uzytkownicy/{id}
        // Zwraca podstawowe dane użytkownika na podstawie jego ID
        [HttpGet("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
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

                // Jeśli użytkownik nie istnieje → 404
                if (user == null)
                    return NotFound("Nie znaleziono użytkownika o podanym ID.");

                return Ok(user); // Zwrócenie danych użytkownika
            }
            catch (Exception ex)
            {
                // Obsługa błędów serwera
                Console.Error.WriteLine($"❌ Błąd w GetById: {ex.Message}");
                return StatusCode(500, "Wystąpił błąd podczas pobierania danych użytkownika.");
            }
        }

    }
}
