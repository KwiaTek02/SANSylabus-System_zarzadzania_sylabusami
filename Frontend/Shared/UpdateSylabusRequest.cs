using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace Frontend.Shared
{
    public class UpdateSylabusRequest
    {
        [Required(ErrorMessage = "Nie zostawiaj tego pustego ćwoku")]
        public string OpisZmiany { get; set; } = default!;        // tekst podany przez wykładowcę
        public DateTime? DataPowstania { get; set; }
        public JsonNode? TresciKsztalcenia { get; set; }
        public JsonNode? EfektyKsztalcenia { get; set; }
        public JsonNode? KryteriaOceny { get; set; }        // tu
        public JsonNode? MetodyWeryfikacji { get; set; }
        public JsonNode? NakladPracy { get; set; }
        public JsonNode? Literatura { get; set; }
        public JsonNode? MetodyRealizacji { get; set; }

        // … pozostałe JSON-owe
        public string? NazwaJednostkiOrganizacyjnej { get; set; }
        public string? ProfilKsztalcenia { get; set; }
        public string? NazwaSpecjalnosci { get; set; }
        public string? RodzajModuluKsztalcenia { get; set; }
        public string? WymaganiaWstepne { get; set; }
        public string? RokData { get; set; }
    }
}
