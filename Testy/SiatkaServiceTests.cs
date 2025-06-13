using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SANSylabusApi.DTOs;
using SylabusAPI.Data;
using SylabusAPI.Models;
using SylabusAPI.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SylabusAPI.DTOs;
using Xunit;

namespace SylabusAPI.Tests
{
    public class SiatkaServiceTests
    {
        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<siatka_przedmiotow, SiatkaPrzedmiotowDto>()
                    .ForMember(dest => dest.PrzedmiotId, opt => opt.MapFrom(src => src.przedmiot_id));
            });
            return config.CreateMapper();
        }

        private SiatkaService GetService(out SyllabusContext db)
        {
            var options = new DbContextOptionsBuilder<SyllabusContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            db = new SyllabusContext(options);
            return new SiatkaService(db, GetMapper());
        }

        [Fact]
        public async Task GetByPrzedmiotAsync_ReturnsCorrectItems()
        {
            var service = GetService(out var db);

            db.siatka_przedmiotows.AddRange(
                new siatka_przedmiotow { id = 1, przedmiot_id = 10, typ = "obowiazkowy", wyklad = 20 },
                new siatka_przedmiotow { id = 2, przedmiot_id = 10, typ = "obowiazkowy", wyklad = 30 },
                new siatka_przedmiotow { id = 3, przedmiot_id = 11, typ = "fakultatywny", wyklad = 40 }
            );
            await db.SaveChangesAsync();

            var result = await service.GetByPrzedmiotAsync(10, "obowiazkowy");

            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal(10, r.PrzedmiotId));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesRecord_WhenExists()
        {
            var service = GetService(out var db);

            db.siatka_przedmiotows.Add(new siatka_przedmiotow
            {
                id = 5,
                przedmiot_id = 20,
                typ = "obowiazkowy",
                wyklad = 10
            });
            await db.SaveChangesAsync();

            var request = new UpdateSiatkaRequest
            {
                Wyklad = 25,
                Cwiczenia = 15,
                Konwersatorium = 5,
                Laboratorium = 10,
                Warsztaty = 0,
                Projekt = 0,
                Seminarium = 0,
                Konsultacje = 3,
                Egzaminy = 2
                
            };

            var result = await service.UpdateAsync(5, request);
            var updated = await db.siatka_przedmiotows.FindAsync(5);

            Assert.True(result);
            //Assert.Equal(25, updated!.wyklad);
            //Assert.Equal(60, updated.sumagodzin);

            Assert.Equal(request.SumaGodzin, updated.sumagodzin);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
        {
            var service = GetService(out var _);
            var result = await service.UpdateAsync(999, new UpdateSiatkaRequest());
            Assert.False(result);
        }
    }
}
