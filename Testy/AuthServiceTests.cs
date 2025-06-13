using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Models;
using SylabusAPI.Services.Implementations;
using SylabusAPI.Settings;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SylabusAPI.Tests
{
    public class AuthServiceTests
    {
        private AuthService GetService(out SyllabusContext context)
        {
            var options = new DbContextOptionsBuilder<SyllabusContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            context = new SyllabusContext(options);

            var jwtOptions = Options.Create(new JwtSettings
            {
                SecretKey = "test-secret-12345678901234567890", // min. 32 znaki
                Issuer = "test",
                Audience = "test",
                ExpiryMinutes = 60
            });

            return new AuthService(context, jwtOptions);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenValidData()
        {
            var service = GetService(out var db);

            var request = new RegisterRequest
            {
                ImieNazwisko = "Jan Testowy",
                Login = "jtest",
                Email = "jan@test.com",
                Password = "Haslo1",
                TypKonta = "gosc"
            };

            var result = await service.RegisterAsync(request);

            Assert.NotNull(result.Token);
            Assert.True(await db.uzytkownicies.AnyAsync(u => u.login == "jtest"));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenLoginExists()
        {
            var service = GetService(out var db);

            db.uzytkownicies.Add(new uzytkownicy
            {
                imie_nazwisko = "Jan Kowalski",
                login = "duplikat",
                email = "mail@mail.com",
                haslo = "hash",
                sol = "sol",
                typ_konta = "gosc"
            });
            await db.SaveChangesAsync();

            var request = new RegisterRequest
            {
                ImieNazwisko = "Ktoś",
                Login = "duplikat",
                Email = "nowy@mail.com",
                Password = "Haslo1",
                TypKonta = "gosc"
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.RegisterAsync(request));
            Assert.Contains("login", ex.Message);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenPasswordWeak()
        {
            var service = GetService(out var db);

            var request = new RegisterRequest
            {
                ImieNazwisko = "Test",
                Login = "testuser",
                Email = "test@mail.com",
                Password = "slabehaslo", // brak cyfry i dużej litery
                TypKonta = "gosc"
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.RegisterAsync(request));
            Assert.Contains("Hasło", ex.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCorrectCredentials()
        {
            var service = GetService(out var db);

            var req = new RegisterRequest
            {
                ImieNazwisko = "Jan",
                Login = "janek",
                Email = "jan@ok.pl",
                Password = "Haslo1",
                TypKonta = "gosc"
            };

            await service.RegisterAsync(req);

            var token = await service.LoginAsync(new LoginRequest
            {
                Login = "janek",
                Password = "Haslo1"
            });

            Assert.NotNull(token.Token);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenWrongPassword()
        {
            var service = GetService(out var db);

            var req = new RegisterRequest
            {
                ImieNazwisko = "Jan",
                Login = "janek2",
                Email = "jan2@ok.pl",
                Password = "Haslo1",
                TypKonta = "gosc"
            };

            await service.RegisterAsync(req);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.LoginAsync(new LoginRequest
                {
                    Login = "janek2",
                    Password = "zleHaslo"
                }));

            Assert.Contains("login", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
