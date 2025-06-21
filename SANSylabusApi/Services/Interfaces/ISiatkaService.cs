using SANSylabusApi.DTOs;
using SylabusAPI.DTOs;

namespace SylabusAPI.Services.Interfaces
{
    public interface ISiatkaService
    {
        // Asynchroniczna metoda zwracająca listę przedmiotów w siatce na podstawie identyfikatora przedmiotu i typu (np. "obowiązkowy", "fakultatywny")
        Task<IEnumerable<SiatkaPrzedmiotowDto>> GetByPrzedmiotAsync(int przedmiotId, string typ);

        // Asynchroniczna metoda aktualizująca dane siatki przedmiotów na podstawie identyfikatora oraz danych z żądania
        // Zwraca true, jeśli aktualizacja się powiodła; w przeciwnym razie false
        Task<bool> UpdateAsync(int id, UpdateSiatkaRequest req);
    }
}
