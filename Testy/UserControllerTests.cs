using Xunit;
using SylabusAPI.Controllers;
using SylabusAPI.Data;
using SylabusAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace SylabusAPI.Tests
{
    public class UserControllerTests
    {
        private SyllabusContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<SyllabusContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            return new SyllabusContext(options);
        }

        [Fact]
        public async Task GetNameByLogin_ReturnsFullName_WhenUserExists()
        {
            // Arrange
            var db = GetInMemoryDb();
            db.uzytkownicies.Add(new uzytkownicy
            {
                id = 1,
                login = "jdoe",
                imie_nazwisko = "Jan Doe",
                tytul = "dr",
                email = "jdoe@example.com",
                haslo = "hashed",
                sol = "salt",
                typ_konta = "admin"
            });
            await db.SaveChangesAsync();

            var controller = new UserController(db);

            // Act
            var result = await controller.GetNameByLogin("jdoe");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("dr Jan Doe", okResult.Value);
        }


        [Fact]
        public async Task GetNameByLogin_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var db = GetInMemoryDb();
            var controller = new UserController(db);

            // Act
            var result = await controller.GetNameByLogin("brak");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Nie znaleziono użytkownika.", notFound.Value);
        }
        [Fact]
        public async Task GetById_ReturnsUser_WhenExists()
        {
            // Arrange
            var db = GetInMemoryDb();
            db.uzytkownicies.Add(new uzytkownicy
            {
                id = 2,
                imie_nazwisko = "Anna Nowak",
                tytul = "",
                login = "anowak",
                email = "a@a.pl",
                haslo = "pass",
                sol = "salt",
                typ_konta = "gosc"
            });
            await db.SaveChangesAsync();

            var controller = new UserController(db);

            // Act
            var result = await controller.GetById(2);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
            var obj = System.Text.Json.JsonDocument.Parse(json).RootElement;

            Assert.Equal(2, obj.GetProperty("Id").GetInt32());
            Assert.Equal("Anna Nowak", obj.GetProperty("ImieNazwisko").GetString());
            Assert.Equal("", obj.GetProperty("Tytul").GetString());
        }


        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var db = GetInMemoryDb();
            var controller = new UserController(db);

            var result = await controller.GetById(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Nie znaleziono użytkownika o podanym ID.", notFound.Value);
        }
    }
}
