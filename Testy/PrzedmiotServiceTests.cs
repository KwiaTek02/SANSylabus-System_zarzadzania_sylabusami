using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Models;
using SylabusAPI.Services.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System;

namespace SylabusAPI.Tests
{
    public class PrzedmiotServiceTests
    {
        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<przedmioty, PrzedmiotDto>();
            });
            return config.CreateMapper();
        }

        private PrzedmiotService GetService(out SyllabusContext context)
        {
            var options = new DbContextOptionsBuilder<SyllabusContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new SyllabusContext(options);
            var mapper = GetMapper();
            return new PrzedmiotService(context, mapper);
        }

        [Fact]
        public async Task GetByKierunekAsync_ReturnsItems_WhenExist()
        {
            var service = GetService(out var db);

            db.przedmioties.AddRange(
                new przedmioty { id = 1, nazwa = "Matematyka", kierunek = "Informatyka", semestr = 1 },
                new przedmioty { id = 2, nazwa = "Fizyka", kierunek = "Informatyka", semestr = 2 },
                new przedmioty { id = 3, nazwa = "Historia", kierunek = "Filologia", semestr = 1 }
            );
            await db.SaveChangesAsync();

            var result = await service.GetByKierunekAsync("Informatyka");

            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Nazwa == "Matematyka");
        }

        [Fact]
        public async Task GetByKierunekAsync_ReturnsEmpty_WhenNoneExist()
        {
            var service = GetService(out _);
            var result = await service.GetByKierunekAsync("Biotechnologia");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsItem_WhenExists()
        {
            var service = GetService(out var db);

            db.przedmioties.Add(new przedmioty
            {
                id = 10,
                nazwa = "Algorytmy",
                kierunek = "Informatyka",
                semestr = 3
            });
            await db.SaveChangesAsync();

            var result = await service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal("Algorytmy", result!.Nazwa);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var service = GetService(out _);
            var result = await service.GetByIdAsync(999);

            Assert.Null(result);
        }
    }
}
