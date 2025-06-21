using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SANSylabusApi.DTOs;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Services.Implementations
{
    public class SiatkaService : ISiatkaService
    {
        private readonly SyllabusContext _db; // Kontekst bazy danych
        private readonly IMapper _mapper;     // Mapper do konwersji encji na DTO

        // Konstruktor z wstrzykiwaniem zależności: kontekst bazy danych i mapper
        public SiatkaService(SyllabusContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // Pobiera listę wpisów siatki przedmiotów na podstawie ID przedmiotu i typu zajęć (np. "obowiązkowe", "fakultatywne")
        public async Task<IEnumerable<SiatkaPrzedmiotowDto>> GetByPrzedmiotAsync(int przedmiotId, string typ)
        {
            var list = await _db.siatka_przedmiotows
                .Where(s => s.przedmiot_id == przedmiotId && s.typ == typ) // filtruje rekordy po ID przedmiotu i typie
                .ToListAsync(); // wykonuje zapytanie do bazy danych asynchronicznie

            // Mapowanie wyników do DTO
            return _mapper.Map<IEnumerable<SiatkaPrzedmiotowDto>>(list);
        }

        // Aktualizuje istniejący rekord siatki przedmiotów na podstawie ID i przekazanych danych z formularza
        public async Task<bool> UpdateAsync(int id, UpdateSiatkaRequest req)
        {
            var siatka = await _db.siatka_przedmiotows.FindAsync(id); // szuka rekordu siatki po ID
            if (siatka == null)
                return false; // jeśli nie znaleziono – zwraca false

            // Przypisanie nowych wartości z requestu do pola rekordu
            siatka.wyklad = req.Wyklad;
            siatka.cwiczenia = req.Cwiczenia;
            siatka.konwersatorium = req.Konwersatorium;
            siatka.laboratorium = req.Laboratorium;
            siatka.warsztaty = req.Warsztaty;
            siatka.projekt = req.Projekt;
            siatka.seminarium = req.Seminarium;
            siatka.konsultacje = req.Konsultacje;
            siatka.egzaminy = req.Egzaminy;
            siatka.sumagodzin = req.SumaGodzin;

            // Zapisuje zmiany do bazy danych
            await _db.SaveChangesAsync();
            return true; // Zwraca true – aktualizacja zakończona powodzeniem
        }
    }
}
