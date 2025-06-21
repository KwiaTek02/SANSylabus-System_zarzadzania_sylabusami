using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Services.Implementations
{
    public class PrzedmiotService : IPrzedmiotService
    {
        private readonly SyllabusContext _db; // Kontekst bazy danych
        private readonly IMapper _mapper;     // Mapper do konwersji encji na DTO

        // Konstruktor z wstrzykiwaniem zależności: kontekst bazy danych i mapper
        public PrzedmiotService(SyllabusContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // Pobiera listę przedmiotów przypisanych do danego kierunku, posortowanych po semestrze
        public async Task<IEnumerable<PrzedmiotDto>> GetByKierunekAsync(string kierunek)
        {
            try
            {
                var items = await _db.przedmioties
                    .Where(p => p.kierunek == kierunek)     // filtracja po kierunku
                    .OrderBy(p => p.semestr)                // sortowanie po semestrze
                    .ToListAsync();                         // wykonanie zapytania asynchronicznie

                // Mapowanie encji na DTO
                return _mapper.Map<IEnumerable<PrzedmiotDto>>(items);
            }
            catch (Exception ex)
            {
                // Obsługa błędu bazy danych
                Console.Error.WriteLine($"❌ Błąd bazy danych (GetByKierunekAsync): {ex.Message}");
                return Enumerable.Empty<PrzedmiotDto>(); // Zwraca pustą listę w razie błędu
            }
        }

        // Pobiera pojedynczy przedmiot po jego ID
        public async Task<PrzedmiotDto?> GetByIdAsync(int id)
        {
            try
            {
                var item = await _db.przedmioties.FindAsync(id); // próba znalezienia przedmiotu po ID
                // Zwróć null jeśli nie znaleziono, w przeciwnym razie zamapuj do DTO
                return item == null ? null : _mapper.Map<PrzedmiotDto>(item);
            }
            catch (Exception ex)
            {
                // Obsługa błędu bazy danych
                Console.Error.WriteLine($"❌ Błąd bazy danych (GetByIdAsync): {ex.Message}");
                return null; // Zwraca null w razie błędu
            }
        }
    }
}
