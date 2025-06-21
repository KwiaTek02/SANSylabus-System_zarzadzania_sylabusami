using SylabusAPI.DTOs;                 
using System.Collections.Generic;     
using System.Threading.Tasks;         

namespace SylabusAPI.Services.Interfaces
{
    // Definicja interfejsu dla serwisu obsługującego historię sylabusa
    public interface IHistoriaService
    {
        // Metoda asynchroniczna zwracająca kolekcję obiektów historii sylabusa na podstawie identyfikatora sylabusa
        Task<IEnumerable<SylabusHistoriaDto>> GetBySylabusAsync(int sylabusId);
    }
}
