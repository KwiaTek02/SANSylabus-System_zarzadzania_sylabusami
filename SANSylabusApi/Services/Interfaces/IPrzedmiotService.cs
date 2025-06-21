using SylabusAPI.DTOs;

namespace SylabusAPI.Services.Interfaces
{
    // Interfejs definiujący kontrakt dla serwisu obsługującego dane o przedmiotach
    public interface IPrzedmiotService
    {
        // Asynchroniczna metoda zwracająca listę przedmiotów przypisanych do danego kierunku studiów
        Task<IEnumerable<PrzedmiotDto>> GetByKierunekAsync(string kierunek);

        // Asynchroniczna metoda zwracająca szczegóły przedmiotu na podstawie jego identyfikatora
        // Może zwrócić null, jeśli przedmiot nie zostanie znaleziony
        Task<PrzedmiotDto?> GetByIdAsync(int id);
    }
}
