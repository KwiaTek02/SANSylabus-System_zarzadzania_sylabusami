using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    }
}