namespace SylabusAPI.DTOs
{
    public class SiatkaPrzedmiotowDto
    {
        public int Id { get; set; }
        public int PrzedmiotId { get; set; }
        public string Typ { get; set; } = default!; // "stacjonarne" lub "niestacjonarne"
        public int? Wyklad { get; set; }
        public int? Cwiczenia { get; set; }
        public int? Konwersatorium { get; set; }
        public int? Laboratorium { get; set; }
        public int? Warsztaty { get; set; }
        public int? Projekt { get; set; }
        public int? Seminarium { get; set; }
        public int? Konsultacje { get; set; }
        public int? Egzaminy { get; set; }
        public int? SumaGodzin { get; set; }
    }
}