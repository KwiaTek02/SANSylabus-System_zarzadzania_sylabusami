using SylabusAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SylabusAPI.Services.Interfaces
{
    public interface IHistoriaService
    {
        Task<IEnumerable<SylabusHistoriaDto>> GetBySylabusAsync(int sylabusId);
    }
}