using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SANSylabusApi.DTOs;
using SylabusAPI.Data;
using SylabusAPI.DTOs;
using SylabusAPI.Models;
using SylabusAPI.Services.Interfaces;


namespace SylabusAPI.Services.Implementations
{
    // Implementacja serwisu odpowiedzialnego za operacje na sylabusach
    public class SylabusService : ISylabusService
    {
        private readonly SyllabusContext _db; // Kontekst bazy danych
        private readonly IMapper _mapper; // Mapper DTO dla model
        private readonly IHttpContextAccessor _httpContext; // Dostęp do kontekstu HTTP
        TimeZoneInfo polandZone; // Strefa czasowa Polski

        // Konstruktor z wstrzykiwaniem zależności
        public SylabusService(SyllabusContext db, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _db = db;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        // Pobierz listę sylabusów przypisanych do danego przedmiotu
        public async Task<IEnumerable<SylabusDto>> GetByPrzedmiotAsync(int przedmiotId)
        {
            var list = await _db.sylabusies
                .Where(s => s.przedmiot_id == przedmiotId)
                .OrderBy(s => s.data_powstania)
                .ToListAsync();

            return list.Select(MapToDto); // Mapowanie do DTO
        }

        // Pobierz sylabus po ID
        public async Task<SylabusDto?> GetByIdAsync(int id)
        {
            var entity = await _db.sylabusies.FindAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        // Utwórz nowy sylabus
        public async Task<SylabusDto> CreateAsync(SylabusDto dto)
        {
            // Pobierz ID użytkownika z tokenu
            var userIdClaim = _httpContext.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Brak poprawnego identyfikatora użytkownika w tokenie.");

            // Utwórz nowy obiekt sylabusu z danymi z DTO
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
                data_powstania = dto.DataPowstania ?? GetPolandNow(),
                kto_stworzyl = userId,
                tresci_ksztalcenia_json = dto.TresciKsztalcenia?.ToJsonString(),
                efekty_ksztalcenia_json = dto.EfektyKsztalcenia?.ToJsonString(),
                metody_weryfikacji_json = dto.MetodyWeryfikacji?.ToJsonString(),
                kryteria_oceny_json = dto.KryteriaOceny?.ToJsonString(),
                naklad_pracy_json = dto.NakladPracy?.ToJsonString(),
                literatura_json = dto.Literatura?.ToJsonString(),
                metody_realizacji_json = dto.MetodyRealizacji?.ToJsonString()
            };

            _db.sylabusies.Add(entity);
            await _db.SaveChangesAsync();

            // Szukamy poprzedniego sylabusu, aby odziedziczyć koordynatorów
            var previous = await _db.sylabusies
                .Where(s => s.przedmiot_id == dto.PrzedmiotId && s.id != entity.id)
                .OrderByDescending(s => s.rok_data)
                .FirstOrDefaultAsync();

            if (previous != null)
            {
                // Kopiuj koordynatorów z poprzedniego sylabusu
                var oldKoordynatorzy = await _db.koordynatorzy_sylabusus
                    .Where(k => k.sylabus_id == previous.id)
                    .ToListAsync();

                foreach (var old in oldKoordynatorzy)
                {
                    var copy = new koordynatorzy_sylabusu
                    {
                        sylabus_id = entity.id,
                        uzytkownik_id = old.uzytkownik_id
                    };
                    _db.koordynatorzy_sylabusus.Add(copy);
                }
            }
            else
            {
                // Jeśli nie ma wcześniejszego sylabusu – autor zostaje koordynatorem
                var fallbackKoordynator = new koordynatorzy_sylabusu
                {
                    sylabus_id = entity.id,
                    uzytkownik_id = userId
                };
                _db.koordynatorzy_sylabusus.Add(fallbackKoordynator);
            }

            await _db.SaveChangesAsync();
            return MapToDto(entity);
        }

        // Aktualizacja sylabusu z wersjonowaniem i historią zmian
        public async Task UpdateAsync(int id, UpdateSylabusRequest req)
        {
            var entity = await _db.sylabusies.FindAsync(id)
                         ?? throw new KeyNotFoundException("Sylabus nie został znaleziony.");

            // Pobierz ID użytkownika
            var userIdClaim = _httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Brak identyfikatora użytkownika w tokenie.");

            var userId = int.Parse(userIdClaim);

            // Sprawdzenie uprawnień użytkownika (admin lub koordynator)
            var isAdmin = _httpContext.HttpContext.User.IsInRole("admin");
            if (!isAdmin)
            {
                var isCoordinator = await _db.koordynatorzy_sylabusus
                    .AnyAsync(k => k.sylabus_id == id && k.uzytkownik_id == userId);
                if (!isCoordinator)
                    throw new UnauthorizedAccessException("Nie jesteś koordynatorem tego sylabusu.");
            }

            // Zwiększenie wersji sylabusu (np. z v1 na v2)
            var oldVer = entity.wersja;
            int oldNum = 1;
            if (oldVer?.StartsWith("v") == true && int.TryParse(oldVer[1..], out var n))
                oldNum = n;
            var newVer = $"v{oldNum + 1}";
            entity.wersja = newVer;

            // Nadpisywanie pól nowymi wartościami (jeśli są)
            entity.data_powstania = req.DataPowstania ?? entity.data_powstania;
            entity.tresci_ksztalcenia_json = req.TresciKsztalcenia?.ToJsonString(JsonOptions()) ?? entity.tresci_ksztalcenia_json;
            entity.efekty_ksztalcenia_json = req.EfektyKsztalcenia?.ToJsonString(JsonOptions()) ?? entity.efekty_ksztalcenia_json;
            entity.metody_weryfikacji_json = req.MetodyWeryfikacji?.ToJsonString(JsonOptions()) ?? entity.metody_weryfikacji_json;
            entity.kryteria_oceny_json = req.KryteriaOceny?.ToJsonString(JsonOptions()) ?? entity.kryteria_oceny_json;
            entity.naklad_pracy_json = req.NakladPracy?.ToJsonString(JsonOptions()) ?? entity.naklad_pracy_json;
            entity.literatura_json = req.Literatura?.ToJsonString(JsonOptions()) ?? entity.literatura_json;
            entity.metody_realizacji_json = req.MetodyRealizacji?.ToJsonString(JsonOptions()) ?? entity.metody_realizacji_json;

            entity.nazwa_jednostki_organizacyjnej = req.NazwaJednostkiOrganizacyjnej ?? entity.nazwa_jednostki_organizacyjnej;
            entity.profil_ksztalcenia = req.ProfilKsztalcenia ?? entity.profil_ksztalcenia;
            entity.nazwa_specjalnosci = req.NazwaSpecjalnosci ?? entity.nazwa_specjalnosci;
            entity.rodzaj_modulu_ksztalcenia = req.RodzajModuluKsztalcenia ?? entity.rodzaj_modulu_ksztalcenia;
            entity.wymagania_wstepne = req.WymaganiaWstepne ?? entity.wymagania_wstepne;
            entity.rok_data = req.RokData ?? entity.rok_data;

            // Ustal aktualny czas w strefie Polski
            try { polandZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"); }
            catch { polandZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw"); }

            var nowPL = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandZone);

            // Zapisz historię zmian
            var history = new sylabus_historium
            {
                sylabus_id = id,
                data_zmiany = DateOnly.FromDateTime(DateTime.UtcNow),
                czas_zmiany = nowPL,
                zmieniony_przez = userId,
                opis_zmiany = req.OpisZmiany,
                wersja_wtedy = oldVer
            };

            _db.sylabusies.Update(entity);
            _db.sylabus_historia.Add(history);
            await _db.SaveChangesAsync();
        }

        // Pobierz koordynatora przypisanego do sylabusu
        public async Task<KoordynatorDto?> GetKoordynatorBySylabusIdAsync(int sylabusId)
        {
            var rekord = await _db.koordynatorzy_sylabusus
                .Include(k => k.uzytkownik)
                .FirstOrDefaultAsync(k => k.sylabus_id == sylabusId);

            if (rekord == null) return null;

            return new KoordynatorDto
            {
                Id = rekord.uzytkownik.id,
                ImieNazwisko = rekord.uzytkownik.imie_nazwisko,
                Tytul = rekord.uzytkownik.tytul
            };
        }

        // Usuń sylabus (z kontrolą uprawnień)
        public async Task DeleteAsync(int id)
        {
            var entity = await _db.sylabusies.FindAsync(id)
                         ?? throw new KeyNotFoundException("Sylabus nie został znaleziony.");

            var userIdClaim = _httpContext.HttpContext!
                .User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userId = int.Parse(userIdClaim);

            var isAdmin = _httpContext.HttpContext.User.IsInRole("admin");
            if (!isAdmin)
            {
                var isCoordinator = await _db.koordynatorzy_sylabusus
                    .AnyAsync(k => k.sylabus_id == id && k.uzytkownik_id == userId);
                if (!isCoordinator)
                    throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tego sylabusu.");
            }

            _db.sylabusies.Remove(entity);
            await _db.SaveChangesAsync();
        }

        // Mapowanie modelu sylabusu na DTO
        private SylabusDto MapToDto(sylabusy s)
        {
            var koordynatorzy = _db.koordynatorzy_sylabusus
                .Where(k => k.sylabus_id == s.id)
                .Include(k => k.uzytkownik)
                .Select(k => k.uzytkownik.tytul + " " + k.uzytkownik.imie_nazwisko)
                .ToList();

            var autor = _db.uzytkownicies
                .Where(u => u.id == s.kto_stworzyl)
                .Select(u => (u.tytul != null ? u.tytul + " " : "") + u.imie_nazwisko)
                .FirstOrDefault();

            return new SylabusDto
            {
                Id = s.id,
                PrzedmiotId = s.przedmiot_id,
                NazwaJednostkiOrganizacyjnej = s.nazwa_jednostki_organizacyjnej,
                ProfilKsztalcenia = s.profil_ksztalcenia,
                NazwaSpecjalnosci = s.nazwa_specjalnosci,
                RodzajModuluKsztalcenia = s.rodzaj_modulu_ksztalcenia,
                WymaganiaWstepne = s.wymagania_wstepne,
                RokData = s.rok_data,
                Wersja = s.wersja,
                DataPowstania = s.data_powstania,
                KtoStworzyl = s.kto_stworzyl,
                StworzylImieNazwiskoTytul = autor,
                TresciKsztalcenia = SafeParseJson(s.tresci_ksztalcenia_json),
                EfektyKsztalcenia = SafeParseJson(s.efekty_ksztalcenia_json),
                MetodyWeryfikacji = SafeParseJson(s.metody_weryfikacji_json),
                KryteriaOceny = SafeParseJson(s.kryteria_oceny_json),
                NakladPracy = SafeParseJson(s.naklad_pracy_json),
                Literatura = SafeParseJson(s.literatura_json),
                MetodyRealizacji = SafeParseJson(s.metody_realizacji_json),
                Koordynatorzy = koordynatorzy
            };
        }

        // Sprawdź czy dany użytkownik jest koordynatorem sylabusu
        public async Task<bool> IsKoordynatorAsync(int sylabusId, int userId)
        {
            return await _db.koordynatorzy_sylabusus
                .AnyAsync(k => k.sylabus_id == sylabusId && k.uzytkownik_id == userId);
        }

        // Bezpieczne parsowanie JSON
        private static JsonNode SafeParseJson(string? json)
        {
            try
            {
                return JsonNode.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json) ?? new JsonObject();
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"⚠️ Błąd parsowania JSON: {ex.Message}\nZawartość: {json}");

                return new JsonObject
                {
                    ["__invalid__"] = true,
                    ["__error__"] = ex.Message
                };
            }
        }

        // Pobranie aktualnego czasu w strefie czasu Polski
        private static DateTime GetPolandNow()
        {
            try
            {
                var polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"); // Windows
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                var polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw"); // Linux/macOS
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone);
            }
        }

        // Ustawienia do serializacji JSON z mniej restrykcyjnym escapowaniem
        private static JsonSerializerOptions JsonOptions() => new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
