using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace SylabusAPI.Controllers
{
    [ApiController]
    [Route("api/sylabusy/{sylabusId}/historia")]
    public class HistoriaController : ControllerBase
    {
        private readonly IHistoriaService _svc;
        public HistoriaController(IHistoriaService svc) => _svc = svc;

        // Dowolny (również anonimowy) może przeglądać historię
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(int sylabusId)
        {
            var list = await _svc.GetBySylabusAsync(sylabusId);
            return Ok(list);
        }
    }
}