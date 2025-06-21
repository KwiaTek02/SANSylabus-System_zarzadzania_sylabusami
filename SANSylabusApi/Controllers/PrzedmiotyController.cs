using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Controllers
{
    [ApiController]
    // Ustalenie bazowej ścieżki URL: api/przedmioty
    [Route("api/przedmioty")]
    public class PrzedmiotyController : ControllerBase
    {
        // Serwis odpowiedzialny za logikę operacji na przedmiotach
        private readonly IPrzedmiotService _svc;

        // Konstruktor z wstrzyknięciem serwisu IPrzedmiotService
        public PrzedmiotyController(IPrzedmiotService svc) => _svc = svc;

        // Endpoint do pobierania listy przedmiotów przypisanych do danego kierunku
        // GET: api/przedmioty/kierunek/Informatyka
        [HttpGet("kierunek/{kierunek}")]
        public async Task<IActionResult> GetByKierunek(string kierunek)
        {
            // Pobranie przedmiotów przypisanych do wskazanego kierunku studiów
            var list = await _svc.GetByKierunekAsync(kierunek);

            // Zwrócenie listy przedmiotów jako odpowiedź HTTP 200 OK
            return Ok(list);
        }

        // Endpoint do pobierania pojedynczego przedmiotu na podstawie jego ID
        // GET: api/przedmioty/42
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // Pobranie przedmiotu o wskazanym ID
            var item = await _svc.GetByIdAsync(id);

            // Jeśli nie znaleziono — HTTP 404 Not Found, w przeciwnym razie — HTTP 200 OK z danymi
            return item is null ? NotFound() : Ok(item);
        }
    }
}
