using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Models;
using SylabusAPI.Settings;
using SylabusAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace SylabusAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly SyllabusContext _db; // Kontekst bazy danych
        private readonly JwtSettings _jwt; // Ustawienia JWT

        // Konstruktor z wstrzykiwaniem zależności
        public AuthService(SyllabusContext db, IOptions<JwtSettings> jwtOptions)
        {
            _db = db;
            _jwt = jwtOptions.Value;
        }

        // Rejestracja nowego użytkownika
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Sprawdzenie, czy login jest już zajęty
            if (await _db.uzytkownicies.AnyAsync(u => u.login == request.Login))
                throw new Exception("Ten login jest już zajęty.");

            // Sprawdzenie, czy email jest już zarejestrowany
            if (await _db.uzytkownicies.AnyAsync(u => u.email == request.Email))
                throw new Exception("Ten adres email jest już zarejestrowany.");

            // Proste sprawdzenie poprawności adresu email
            if (!request.Email.Contains("@"))
                throw new Exception("Adres email jest nieprawidłowy.");

            // Sprawdzenie siły hasła
            if (!HasValidPassword(request.Password))
                throw new Exception("Hasło musi zawierać co najmniej jedną dużą literę i jedną cyfrę.");

            // Wygenerowanie losowej soli
            var saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            // Haszowanie hasła przy użyciu PBKDF2
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32));

            // Utworzenie nowego obiektu użytkownika
            var user = new uzytkownicy
            {
                imie_nazwisko = request.ImieNazwisko,
                tytul = request.Tytul,
                login = request.Login,
                haslo = hash,
                sol = salt,
                email = request.Email,
                typ_konta = request.TypKonta
            };

            // Dodanie użytkownika do bazy danych i zapis zmian
            _db.uzytkownicies.Add(user);
            await _db.SaveChangesAsync();

            // Wygenerowanie i zwrócenie tokenu JWT
            return GenerateToken(user.id, user.login, user.typ_konta);
        }

        // Logowanie użytkownika
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // Pobranie użytkownika z bazy danych na podstawie loginu
            var user = await _db.uzytkownicies.FirstOrDefaultAsync(u => u.login == request.Login);
            if (user is null)
                throw new UnauthorizedAccessException("Nieprawidłowy login lub hasło.");

            // Sprawdzenie, czy konto jest kontem Google
            if (user.sol == "google")
                throw new UnauthorizedAccessException("To konto używa logowania przez Google.");

            // Pobranie soli i przeliczenie hasła
            var saltBytes = Convert.FromBase64String(user.sol!);
            var hashAttempt = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32));

            // Porównanie przeliczonego hasła z hasłem z bazy
            if (hashAttempt != user.haslo)
                throw new UnauthorizedAccessException("Nieprawidłowy login lub hasło.");

            // Wygenerowanie i zwrócenie tokenu JWT
            return GenerateToken(user.id, user.login, user.typ_konta);
        }

        // Generowanie tokenu JWT na podstawie danych użytkownika
        private AuthResponse GenerateToken(int userId, string login, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("login", login),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            // Tworzenie klucza i danych do podpisu
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            // Tworzenie tokenu JWT
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expires
            };
        }

        // Sprawdzenie czy hasło zawiera przynajmniej jedną dużą literę i jedną cyfrę
        private bool HasValidPassword(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsDigit);
        }

        // Logowanie za pomocą Google (lub tworzenie konta jeśli nie istnieje)
        public async Task<AuthResponse> GoogleLoginAsync(string email, string name)
        {
            // Szukanie użytkownika na podstawie adresu email
            var user = await _db.uzytkownicies.FirstOrDefaultAsync(u => u.email == email);

            // Jeśli użytkownik nie istnieje, utwórz nowe konto typu gość
            if (user == null)
            {
                user = new uzytkownicy
                {
                    imie_nazwisko = name,
                    login = email,
                    email = email,
                    tytul = null,
                    haslo = "google-external", // placeholder hasła
                    sol = "google",
                    typ_konta = "gosc" // konto typu gość
                };

                _db.uzytkownicies.Add(user);
                await _db.SaveChangesAsync();
            }

            // Wygenerowanie i zwrócenie tokenu JWT
            return GenerateToken(user.id, user.login, user.typ_konta);
        }

    }
}
