using System;
using System.Text.Json.Nodes;

namespace Frontend.Shared
{
    public class SylabusDto
    {
        public int Id { get; set; }
        public int PrzedmiotId { get; set; }
        public string Wersja { get; set; } = default!;
        public string? NazwaJednostkiOrganizacyjnej { get; set; }
        public string? ProfilKsztalcenia { get; set; }
        public string? NazwaSpecjalnosci { get; set; }
        public string? RodzajModuluKsztalcenia { get; set; }
        public string? WymaganiaWstepne { get; set; }
        public string? RokData { get; set; }
        public DateTime? DataPowstania { get; set; }
        public int KtoStworzyl { get; set; }
        public JsonNode? TresciKsztalcenia { get; set; }
        public JsonNode? EfektyKsztalcenia { get; set; }
        public JsonNode? MetodyWeryfikacji { get; set; }
        public JsonNode? KryteriaOceny { get; set; }
        public JsonNode? NakladPracy { get; set; }
        public JsonNode? Literatura { get; set; }
        public JsonNode? MetodyRealizacji { get; set; }
        public List<string>? Koordynatorzy { get; set; }
    }
}