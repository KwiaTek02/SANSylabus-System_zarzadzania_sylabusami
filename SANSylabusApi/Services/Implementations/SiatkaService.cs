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
        private readonly SyllabusContext _db;
        private readonly IMapper _mapper;

        public SiatkaService(SyllabusContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SiatkaPrzedmiotowDto>> GetByPrzedmiotAsync(int przedmiotId, string typ)
        {
            var list = await _db.siatka_przedmiotows
                .Where(s => s.przedmiot_id == przedmiotId && s.typ == typ)
                .ToListAsync();
            return _mapper.Map<IEnumerable<SiatkaPrzedmiotowDto>>(list);
        }

        public async Task<bool> UpdateAsync(int id, UpdateSiatkaRequest req)
        {
            var siatka = await _db.siatka_przedmiotows.FindAsync(id);
            if (siatka == null)
                return false;

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

            await _db.SaveChangesAsync();
            return true;
        }
    }
}