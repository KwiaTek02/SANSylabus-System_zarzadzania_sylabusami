using SylabusAPI.DTOs;

namespace SylabusAPI.Services.Interfaces
{
    // Interfejs definiujący metody autoryzacyjne, które powinny zostać zaimplementowane przez klasę serwisową
    public interface IAuthService
    {
        // Rejestracja nowego użytkownika na podstawie danych z formularza
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        // Logowanie użytkownika na podstawie danych logowania (np. e-mail i hasło)
        Task<AuthResponse> LoginAsync(LoginRequest request);

        // Logowanie użytkownika przez konto Google, wykorzystując e-mail i nazwę użytkownika z Google
        Task<AuthResponse> GoogleLoginAsync(string email, string name);
    }
}
