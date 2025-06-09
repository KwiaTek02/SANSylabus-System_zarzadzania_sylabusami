using Microsoft.EntityFrameworkCore;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SylabusAPI.Services.Implementations
{
    public class HistoriaService : IHistoriaService
    {
        private readonly SyllabusContext _db;

        public HistoriaService(SyllabusContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SylabusHistoriaDto>> GetBySylabusAsync(int sylabusId)
        {
            var list = await _db.sylabus_historia
                .Include(h => h.zmieniony_przezNavigation)
                .Where(h => h.sylabus_id == sylabusId)
                .OrderByDescending(h => h.czas_zmiany)
                .ToListAsync();

            return list.Select(h => new SylabusHistoriaDto
            {
                Id = h.id,
                SylabusId = h.sylabus_id,
                DataZmiany = h.data_zmiany,
                CzasZmiany = h.czas_zmiany,
                ZmienionyPrzez = h.zmieniony_przez,
                OpisZmiany = h.opis_zmiany,
                WersjaWtedy = h.wersja_wtedy,
                ZmieniajacyImieNazwiskoTytul = $"{(string.IsNullOrWhiteSpace(h.zmieniony_przezNavigation.tytul) ? "" : h.zmieniony_przezNavigation.tytul + " ")}{h.zmieniony_przezNavigation.imie_nazwisko}"
            });
        }
    }
}