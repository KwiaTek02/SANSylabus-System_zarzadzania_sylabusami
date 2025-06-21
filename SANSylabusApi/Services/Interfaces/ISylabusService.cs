using SANSylabusApi.DTOs;
using SylabusAPI.DTOs;

namespace SylabusAPI.Services.Interfaces
{
    public interface ISylabusService
    {
        // Asynchroniczna metoda zwracająca listę sylabusów przypisanych do danego przedmiotu
        Task<IEnumerable<SylabusDto>> GetByPrzedmiotAsync(int przedmiotId);

        // Asynchroniczna metoda pobierająca sylabus na podstawie jego identyfikatora
        // Może zwrócić null, jeśli sylabus nie zostanie znaleziony
        Task<SylabusDto?> GetByIdAsync(int id);

        // Asynchroniczna metoda tworząca nowy sylabus na podstawie danych wejściowych
        Task<SylabusDto> CreateAsync(SylabusDto dto);

        // Task UpdateAsync(int id, SylabusDto dto);

        // Asynchroniczna metoda aktualizująca istniejący sylabus na podstawie identyfikatora i danych żądania
        Task UpdateAsync(int id, UpdateSylabusRequest req);

        // Asynchroniczna metoda usuwająca sylabus na podstawie jego identyfikatora
        Task DeleteAsync(int id);

        // Asynchroniczna metoda pobierająca dane koordynatora przypisanego do sylabusa
        Task<KoordynatorDto?> GetKoordynatorBySylabusIdAsync(int sylabusId);

        // Asynchroniczna metoda sprawdzająca, czy dany użytkownik jest koordynatorem danego sylabusa
        // Zwraca true, jeśli użytkownik jest koordynatorem
        Task<bool> IsKoordynatorAsync(int sylabusId, int userId);
    }
}
