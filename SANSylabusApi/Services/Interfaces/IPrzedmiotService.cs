using SylabusAPI.DTOs;

namespace SylabusAPI.Services.Interfaces
{
    public interface IPrzedmiotService
    {
        Task<IEnumerable<PrzedmiotDto>> GetByKierunekAsync(string kierunek);
        Task<PrzedmiotDto?> GetByIdAsync(int id);
    }
}