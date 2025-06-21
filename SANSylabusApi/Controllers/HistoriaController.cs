using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace SylabusAPI.Controllers
{
    // Oznaczenie klasy jako kontrolera API (automatyczne wiązanie modeli, walidacja, itd.)
    [ApiController]
    // Definiowanie trasy: np. api/sylabusy/5/historia
    [Route("api/sylabusy/{sylabusId}/historia")]
    public class HistoriaController : ControllerBase
    {
        // Serwis odpowiedzialny za logikę historii sylabusa
        private readonly IHistoriaService _svc;

        // Konstruktor z wstrzyknięciem zależności serwisu historii
        public HistoriaController(IHistoriaService svc) => _svc = svc;

        // Endpoint pozwalający na pobranie historii dla danego sylabusa
        // Dostępny również dla niezalogowanych użytkowników (anonimowych)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(int sylabusId)
        {
            // Pobranie listy historii danego sylabusa na podstawie jego ID
            var list = await _svc.GetBySylabusAsync(sylabusId);

            // Zwrócenie listy w odpowiedzi HTTP 200 OK
            return Ok(list);
        }
    }
}
