using Microsoft.EntityFrameworkCore;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SylabusAPI.Services.Implementations
{
    // Implementacja serwisu historii zmian sylabusa
    public class HistoriaService : IHistoriaService
    {
        private readonly SyllabusContext _db; // Kontekst bazy danych

        public HistoriaService(SyllabusContext db)
        {
            _db = db;
        }

        // Pobranie historii zmian dla danego sylabusa (po ID)
        public async Task<IEnumerable<SylabusHistoriaDto>> GetBySylabusAsync(int sylabusId)
        {
            // Pobranie listy rekordów historii dla danego sylabusa,
            // łącznie z informacjami o użytkowniku, który dokonał zmiany
            var list = await _db.sylabus_historia
                .Include(h => h.zmieniony_przezNavigation) // dołączenie danych o zmieniającym użytkowniku
                .Where(h => h.sylabus_id == sylabusId) // filtrowanie po ID sylabusa
                .OrderByDescending(h => h.czas_zmiany) // sortowanie malejąco wg czasu zmiany
                .ToListAsync(); // wykonanie zapytania asynchronicznie

            // Mapowanie danych z modelu bazy danych do obiektu DTO
            return list.Select(h => new SylabusHistoriaDto
            {
                Id = h.id,
                SylabusId = h.sylabus_id,
                DataZmiany = h.data_zmiany,
                CzasZmiany = h.czas_zmiany,
                ZmienionyPrzez = h.zmieniony_przez,
                OpisZmiany = h.opis_zmiany,
                WersjaWtedy = h.wersja_wtedy,

                // Sklejenie tytułu i imienia + nazwiska osoby, która wprowadziła zmianę
                ZmieniajacyImieNazwiskoTytul = $"{(string.IsNullOrWhiteSpace(h.zmieniony_przezNavigation.tytul) ? "" : h.zmieniony_przezNavigation.tytul + " ")}{h.zmieniony_przezNavigation.imie_nazwisko}"
            });
        }
    }
}
