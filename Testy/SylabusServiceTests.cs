using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Models;
using SylabusAPI.Services.Implementations;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SylabusAPI.Tests
{
    public class SylabusServiceTests
    {
        private SyllabusContext GetInMemoryDb(out DbContextOptions<SyllabusContext> options)
        {
            options = new DbContextOptionsBuilder<SyllabusContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SyllabusContext(options);
        }

        private IHttpContextAccessor MockHttpContext(int userId, bool isAdmin = false)
        {
            var context = new DefaultHttpContext();
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            if (isAdmin) claims.Add(new Claim(ClaimTypes.Role, "admin"));

            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(a => a.HttpContext).Returns(context);
            return accessor.Object;
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var db = GetInMemoryDb(out var _);
            var service = new SylabusService(db, null!, MockHttpContext(1));
            var result = await service.GetByIdAsync(123);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_CreatesCorrectly()
        {
            var db = GetInMemoryDb(out var _);
            db.uzytkownicies.Add(new uzytkownicy
            {
                id = 1,
                imie_nazwisko = "Test User",
                tytul = "mgr",
                login = "test",
                email = "test@test.com",
                haslo = "hash",
                sol = "salt",
                typ_konta = "admin"
            });
            await db.SaveChangesAsync();

            var dto = new SylabusDto { PrzedmiotId = 1, Wersja = "v1", RokData = "2024" };
            var service = new SylabusService(db, null!, MockHttpContext(1));

            var result = await service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("v1", result.Wersja);
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenNotCoordinatorOrAdmin()
        {
            var db = GetInMemoryDb(out var _);

            db.sylabusies.Add(new sylabusy
            {
                id = 5,
                kto_stworzyl = 999,
                wersja = "v1" 
            });

            await db.SaveChangesAsync();

            var service = new SylabusService(db, null!, MockHttpContext(1));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.DeleteAsync(5));
        }


        [Fact]
        public async Task IsKoordynatorAsync_ReturnsTrue_WhenUserIsKoordynator()
        {
            var db = GetInMemoryDb(out var _);
            db.koordynatorzy_sylabusus.Add(new koordynatorzy_sylabusu { sylabus_id = 10, uzytkownik_id = 1 });
            await db.SaveChangesAsync();

            var service = new SylabusService(db, null!, MockHttpContext(1));
            var result = await service.IsKoordynatorAsync(10, 1);

            Assert.True(result);
        }
    }
}
