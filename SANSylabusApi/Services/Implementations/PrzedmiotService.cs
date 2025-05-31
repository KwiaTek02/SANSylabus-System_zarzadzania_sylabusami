using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Services.Implementations
{
    public class PrzedmiotService : IPrzedmiotService
    {
        private readonly SyllabusContext _db;
        private readonly IMapper _mapper;

        public PrzedmiotService(SyllabusContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PrzedmiotDto>> GetByKierunekAsync(string kierunek)
        {
            var items = await _db.przedmioties
                .Where(p => p.kierunek == kierunek)
                .OrderBy(p => p.semestr)
                .ToListAsync();
            return _mapper.Map<IEnumerable<PrzedmiotDto>>(items);
        }

        public async Task<PrzedmiotDto?> GetByIdAsync(int id)
        {
            var item = await _db.przedmioties.FindAsync(id);
            return item == null ? null : _mapper.Map<PrzedmiotDto>(item);
        }
    }
}
