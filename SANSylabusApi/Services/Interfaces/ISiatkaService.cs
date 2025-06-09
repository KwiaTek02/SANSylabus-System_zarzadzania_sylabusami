using SANSylabusApi.DTOs;
using SylabusAPI.DTOs;

namespace SylabusAPI.Services.Interfaces
{
    public interface ISiatkaService
    {
        Task<IEnumerable<SiatkaPrzedmiotowDto>> GetByPrzedmiotAsync(int przedmiotId, string typ);
        Task<bool> UpdateAsync(int id, UpdateSiatkaRequest req);
    }
}