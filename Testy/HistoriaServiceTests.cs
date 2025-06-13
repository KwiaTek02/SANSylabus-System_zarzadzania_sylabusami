using Microsoft.EntityFrameworkCore;
using SylabusAPI.Data;
using SylabusAPI.Models;
using SylabusAPI.Services.Implementations;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace SylabusAPI.Tests
{
    public class HistoriaServiceTests
    {
        private HistoriaService GetService(out SyllabusContext context)
        {
            var options = new DbContextOptionsBuilder<SyllabusContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unikalna baza dla każdego testu
                .EnableSensitiveDataLogging()
                .Options;

            context = new SyllabusContext(options);
            return new HistoriaService(context);
        }

        [Fact]
        public async Task GetBySylabusAsync_ReturnsHistoriaList_WhenExists()
        {
            // Arrange
            var service = GetService(out var db);

            var user = new uzytkownicy
            {
                id = 1,
                imie_nazwisko = "Jan Kowalski",
                tytul = "dr",
                login = "jan",
                email = "jan@example.com",
                haslo = "hash",
                sol = "sol",
                typ_konta = "gosc"
            };

            db.uzytkownicies.Add(user);

            db.sylabus_historia.Add(new sylabus_historium
            {
                id = 1,
                sylabus_id = 5,
                data_zmiany = DateOnly.FromDateTime(DateTime.Today),
                czas_zmiany = DateTime.UtcNow,
                zmieniony_przez = user.id,
                zmieniony_przezNavigation = user,
                opis_zmiany = "Dodano punkt 3",
                wersja_wtedy = "v1"
            });

            await db.SaveChangesAsync();

            // Act
            var result = await service.GetBySylabusAsync(5);

            // Assert
            var entry = Assert.Single(result);
            Assert.Equal(1, entry.Id);
            Assert.Equal("dr Jan Kowalski", entry.ZmieniajacyImieNazwiskoTytul);
            Assert.Equal("Dodano punkt 3", entry.OpisZmiany);
            Assert.Equal("v1", entry.WersjaWtedy);
        }

        [Fact]
        public async Task GetBySylabusAsync_ReturnsEmpty_WhenNoEntries()
        {
            // Arrange
            var service = GetService(out var db);

            // Act
            var result = await service.GetBySylabusAsync(999);

            // Assert
            Assert.Empty(result);
        }
    }
}
