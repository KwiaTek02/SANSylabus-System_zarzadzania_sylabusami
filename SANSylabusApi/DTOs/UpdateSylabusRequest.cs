using System.Text.Json.Nodes;

namespace SylabusAPI.DTOs
{
    public class UpdateSylabusRequest
    {
        public string OpisZmiany { get; set; }  
        public DateTime? DataPowstania { get; set; }
        public JsonNode? TresciKsztalcenia { get; set; }
        public JsonNode? EfektyKsztalcenia { get; set; }
        public JsonNode? KryteriaOceny { get; set; }        // tu
        public JsonNode? MetodyWeryfikacji { get; set; }
        public JsonNode? NakladPracy { get; set; }
        public JsonNode? Literatura { get; set; }
        public JsonNode? MetodyRealizacji { get; set; }

        // … pozostałe JSON-owe
        public string? NazwaJednostkiOrganizacyjnej { get; set; } // 
        public string? ProfilKsztalcenia { get; set; }  // 
        public string? NazwaSpecjalnosci { get; set; } //
        public string? RodzajModuluKsztalcenia { get; set; } // 
        public string? WymaganiaWstepne { get; set; } //jest
        public string? RokData { get; set; } // jest
    }
}
