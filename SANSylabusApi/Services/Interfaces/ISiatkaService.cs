using SylabusAPI.DTOs;

namespace SylabusAPI.Services.Interfaces
{
    public interface ISiatkaService
    {
        Task<IEnumerable<SiatkaPrzedmiotowDto>> GetByPrzedmiotAsync(int przedmiotId, string typ);
    }
}