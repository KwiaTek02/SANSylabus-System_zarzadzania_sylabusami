using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SANSylabusApi.DTOs;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Models;
using SylabusAPI.Services.Interfaces;

namespace SylabusAPI.Services.Implementations
{
    public class SylabusService : ISylabusService
    {
        private readonly SyllabusContext _db;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public SylabusService(
            SyllabusContext db,
            IMapper mapper,
            IHttpContextAccessor httpContext)
        {
            _db = db;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<IEnumerable<SylabusDto>> GetByPrzedmiotAsync(int przedmiotId)
        {
            var list = await _db.sylabusies
                .Where(s => s.przedmiot_id == przedmiotId)
                .OrderBy(s => s.data_powstania)
                .ToListAsync();

            return list.Select(MapToDto);
        }

        public async Task<SylabusDto?> GetByIdAsync(int id)
        {
            var entity = await _db.sylabusies.FindAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<SylabusDto> CreateAsync(SylabusDto dto)
        {
            var userIdClaim = _httpContext.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException(
                    "Brak poprawnego identyfikatora użytkownika w tokenie.");

            var entity = new sylabusy
            {
                przedmiot_id = dto.PrzedmiotId,
                wersja = dto.Wersja,

                nazwa_jednostki_organizacyjnej = dto.NazwaJednostkiOrganizacyjnej,
                profil_ksztalcenia = dto.ProfilKsztalcenia,
                nazwa_specjalnosci = dto.NazwaSpecjalnosci,
                rodzaj_modulu_ksztalcenia = dto.RodzajModuluKsztalcenia,
                wymagania_wstepne = dto.WymaganiaWstepne,
                rok_data = dto.RokData,

                data_powstania = dto.DataPowstania ?? DateTime.UtcNow,
                kto_stworzyl = userId,
                tresci_ksztalcenia_json = dto.TresciKsztalcenia?.ToJsonString(),
                efekty_ksztalcenia_json = dto.EfektyKsztalcenia?.ToJsonString(),
                metody_weryfikacji_json = dto.MetodyWeryfikacji?.ToJsonString(),
                kryteria_oceny_json = dto.KryteriaOceny?.ToJsonString(),        //tuu 
                naklad_pracy_json = dto.NakladPracy?.ToJsonString(),
                literatura_json = dto.Literatura?.ToJsonString(),
                metody_realizacji_json = dto.MetodyRealizacji?.ToJsonString()
            };

            _db.sylabusies.Add(entity);
            await _db.SaveChangesAsync();
            return MapToDto(entity);
        }

        /*public async Task UpdateAsync(int id, SylabusDto dto)
        {
            var entity = await _db.sylabusies.FindAsync(id)
                         ?? throw new KeyNotFoundException("Sylabus nie został znaleziony.");

            // Sprawdź, czy cokolwiek się zmieniło:
            bool hasChanges = false;

            if (entity.wersja != dto.Wersja)
            {
                entity.wersja = dto.Wersja;
                hasChanges = true;
            }
            if (entity.nazwa_jednostki_organizacyjnej != dto.NazwaJednostkiOrganizacyjnej)
            {
                entity.nazwa_jednostki_organizacyjnej = dto.NazwaJednostkiOrganizacyjnej;
                hasChanges = true;
            }
            if (entity.profil_ksztalcenia != dto.ProfilKsztalcenia)
            {
                entity.profil_ksztalcenia = dto.ProfilKsztalcenia;
                hasChanges = true;
            }
            if (entity.nazwa_specjalnosci != dto.NazwaSpecjalnosci)
            {
                entity.nazwa_specjalnosci = dto.NazwaSpecjalnosci;
                hasChanges = true;
            }
            if (entity.rodzaj_modulu_ksztalcenia != dto.RodzajModuluKsztalcenia)
            {
                entity.rodzaj_modulu_ksztalcenia = dto.RodzajModuluKsztalcenia;
                hasChanges = true;
            }
            if (entity.wymagania_wstepne != dto.WymaganiaWstepne)
            {
                entity.wymagania_wstepne = dto.WymaganiaWstepne;
                hasChanges = true;
            }
            if (entity.rok_data != dto.RokData)
            {
                entity.rok_data = dto.RokData;
                hasChanges = true;
            }

            // JSON fields
            string toJson(JsonNode? node) => node?.ToJsonString() ?? "{}";

            if (entity.tresci_ksztalcenia_json != toJson(dto.TresciKsztalcenia))
            {
                entity.tresci_ksztalcenia_json = toJson(dto.TresciKsztalcenia);
                hasChanges = true;
            }
            if (entity.efekty_ksztalcenia_json != toJson(dto.EfektyKsztalcenia))
            {
                entity.efekty_ksztalcenia_json = toJson(dto.EfektyKsztalcenia);
                hasChanges = true;
            }
            if (entity.metody_weryfikacji_json != toJson(dto.MetodyWeryfikacji))
            {
                entity.metody_weryfikacji_json = toJson(dto.MetodyWeryfikacji);
                hasChanges = true;
            }
            if (entity.naklad_pracy_json != toJson(dto.NakladPracy))
            {
                entity.naklad_pracy_json = toJson(dto.NakladPracy);
                hasChanges = true;
            }
            if (entity.literatura_json != toJson(dto.Literatura))
            {
                entity.literatura_json = toJson(dto.Literatura);
                hasChanges = true;
            }
            if (entity.metody_realizacji_json != toJson(dto.MetodyRealizacji))
            {
                entity.metody_realizacji_json = toJson(dto.MetodyRealizacji);
                hasChanges = true;
            }

            if (!hasChanges)
                return; // nic nowego do zapisania

            // Zapisz zmiany sylabusu
            _db.sylabusies.Update(entity);

            // Dodaj wpis do historii
            var userIdClaim = _httpContext.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
            int userId = int.TryParse(userIdClaim, out var uid) ? uid : entity.kto_stworzyl;

            var history = new sylabus_historium
            {
                sylabus_id = id,
                data_zmiany = DateOnly.FromDateTime(DateTime.UtcNow),
                czas_zmiany = DateTime.UtcNow,
                zmieniony_przez = userId,
                opis_zmiany = $"Zaktualizowano sylabus do wersji {dto.Wersja}",
                wersja_wtedy = dto.Wersja
            };
            _db.sylabus_historia.Add(history);

            await _db.SaveChangesAsync();
        }*/

        public async Task UpdateAsync(int id, UpdateSylabusRequest req)
        {
            

            // 1) Załaduj sylabus
            var entity = await _db.sylabusies.FindAsync(id)
                         ?? throw new KeyNotFoundException("Sylabus nie został znaleziony.");

            // 2) Pobierz ID użytkownika z tokena
            var userIdClaim = _httpContext.HttpContext!
                .User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(userIdClaim);

            // 3) Sprawdź uprawnienia: albo admin, albo koordynator
            var isAdmin = _httpContext.HttpContext.User.IsInRole("admin");
            if (!isAdmin)
            {
                var isCoordinator = await _db.koordynatorzy_sylabusus
                    .AnyAsync(k => k.sylabus_id == id && k.uzytkownik_id == userId);
                if (!isCoordinator)
                    throw new UnauthorizedAccessException("Nie jesteś koordynatorem tego sylabusu.");
            }






            // 1) Wyciągnij poprzednią wersję, np. "v1" → 1
            var oldVer = entity.wersja; // np. "v1"
            int oldNum = 1;
            if (oldVer?.StartsWith("v") == true && int.TryParse(oldVer[1..], out var n))
                oldNum = n;

            // 2) Ustaw nową wersję = oldNum+1
            var newVer = $"v{oldNum + 1}";
            entity.wersja = newVer;

            // 3) Nadpisz resztę pól z req
            entity.data_powstania = req.DataPowstania ?? entity.data_powstania;
            entity.tresci_ksztalcenia_json = req.TresciKsztalcenia?.ToJsonString() ?? entity.tresci_ksztalcenia_json;
            entity.efekty_ksztalcenia_json = req.EfektyKsztalcenia?.ToJsonString() ?? entity.efekty_ksztalcenia_json;
            entity.metody_weryfikacji_json = req.MetodyWeryfikacji?.ToJsonString() ?? entity.metody_weryfikacji_json;
            entity.kryteria_oceny_json = req.KryteriaOceny?.ToJsonString() ?? entity.kryteria_oceny_json;
            entity.naklad_pracy_json = req.NakladPracy?.ToJsonString() ?? entity.naklad_pracy_json;
            entity.literatura_json = req.Literatura?.ToJsonString() ?? entity.literatura_json;
            entity.metody_realizacji_json = req.MetodyRealizacji?.ToJsonString() ?? entity.metody_realizacji_json;


            entity.nazwa_jednostki_organizacyjnej = req.NazwaJednostkiOrganizacyjnej ?? entity.nazwa_jednostki_organizacyjnej;
            entity.profil_ksztalcenia = req.ProfilKsztalcenia ?? entity.profil_ksztalcenia;
            entity.nazwa_specjalnosci = req.NazwaSpecjalnosci ?? entity.nazwa_specjalnosci;
            entity.rodzaj_modulu_ksztalcenia = req.RodzajModuluKsztalcenia ?? entity.rodzaj_modulu_ksztalcenia;
            entity.wymagania_wstepne = req.WymaganiaWstepne ?? entity.wymagania_wstepne;
            entity.rok_data = req.RokData ?? entity.rok_data;

            
            

            var history = new sylabus_historium
            {
                sylabus_id = id,
                data_zmiany = DateOnly.FromDateTime(DateTime.UtcNow),
                czas_zmiany = DateTime.UtcNow,
                zmieniony_przez = userId,
                // opis z req
                opis_zmiany = req.OpisZmiany,
                // a tu poprzednia wersja
                wersja_wtedy = oldVer
            };

            _db.sylabusies.Update(entity);
            _db.sylabus_historia.Add(history);

            await _db.SaveChangesAsync();
        }


        public async Task<KoordynatorDto?> GetKoordynatorBySylabusIdAsync(int sylabusId)
        {
            var rekord = await _db.koordynatorzy_sylabusus
                .Include(k => k.uzytkownik)
                .FirstOrDefaultAsync(k => k.sylabus_id == sylabusId);

            if (rekord == null)
                return null;

            return new KoordynatorDto
            {
                Id = rekord.uzytkownik.id,
                ImieNazwisko = rekord.uzytkownik.imie_nazwisko,
                Tytul = rekord.uzytkownik.tytul
            };
        }


        public async Task DeleteAsync(int id)
        {
            var entity = await _db.sylabusies.FindAsync(id)
                         ?? throw new KeyNotFoundException("Sylabus nie został znaleziony.");

            // Pobranie zalogowanego userId
            var userIdClaim = _httpContext.HttpContext!
                .User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(userIdClaim);

            // Sprawdzenie uprawnień: admin lub koordynator
            var isAdmin = _httpContext.HttpContext.User.IsInRole("admin");
            if (!isAdmin)
            {
                var isCoordinator = await _db.koordynatorzy_sylabusus
                    .AnyAsync(k => k.sylabus_id == id && k.uzytkownik_id == userId);
                if (!isCoordinator)
                    throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tego sylabusu.");
            }

            // Usuń i zapisz zmiany
            _db.sylabusies.Remove(entity);
            await _db.SaveChangesAsync();
        }


        private SylabusDto MapToDto(sylabusy s)
        {
            var koordynatorzy = _db.koordynatorzy_sylabusus
                .Where(k => k.sylabus_id == s.id)
                .Include(k => k.uzytkownik)
                .Select(k => k.uzytkownik.tytul + " " + k.uzytkownik.imie_nazwisko)
                .ToList();

            return new SylabusDto
            {
                Id = s.id,
                PrzedmiotId = s.przedmiot_id,

                // Te pola wcześniej były null – teraz je dodajemy:
                NazwaJednostkiOrganizacyjnej = s.nazwa_jednostki_organizacyjnej,
                ProfilKsztalcenia = s.profil_ksztalcenia,
                NazwaSpecjalnosci = s.nazwa_specjalnosci,
                RodzajModuluKsztalcenia = s.rodzaj_modulu_ksztalcenia,
                WymaganiaWstepne = s.wymagania_wstepne,
                RokData = s.rok_data,

                // Istniejące pola:
                Wersja = s.wersja,
                DataPowstania = s.data_powstania,
                KtoStworzyl = s.kto_stworzyl,
                TresciKsztalcenia = JsonNode.Parse(s.tresci_ksztalcenia_json ?? "{}"),
                EfektyKsztalcenia = JsonNode.Parse(s.efekty_ksztalcenia_json ?? "{}"),
                MetodyWeryfikacji = JsonNode.Parse(s.metody_weryfikacji_json ?? "{}"),
                KryteriaOceny = JsonNode.Parse(s.kryteria_oceny_json ?? "{}"),
                NakladPracy = JsonNode.Parse(s.naklad_pracy_json ?? "{}"),
                Literatura = JsonNode.Parse(s.literatura_json ?? "{}"),
                MetodyRealizacji = JsonNode.Parse(s.metody_realizacji_json ?? "{}"),
                Koordynatorzy = koordynatorzy
            };
        }
    }
}
