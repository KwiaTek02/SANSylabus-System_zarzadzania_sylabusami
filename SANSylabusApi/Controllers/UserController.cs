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
        public async Task<IActionResult> GetNameByLogin([FromQuery] string login)
        {
            var user = await _db.uzytkownicies
                .Where(u => u.login == login)
                .Select(u => new
                {
                    PelneImie = (u.tytul != null ? u.tytul + " " : "") + u.imie_nazwisko
                })
                .FirstOrDefaultAsync();

            return user is not null ? Ok(user.PelneImie) : NotFound("Nie znaleziono użytkownika.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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
                return NotFound();

            return Ok(user);
        }

    }
}