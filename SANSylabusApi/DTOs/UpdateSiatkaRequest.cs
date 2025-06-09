namespace SANSylabusApi.DTOs
{
    public class UpdateSiatkaRequest
    {
        public int Wyklad { get; set; }
        public int Cwiczenia { get; set; }
        public int Konwersatorium { get; set; }
        public int Laboratorium { get; set; }
        public int Warsztaty { get; set; }
        public int Projekt { get; set; }
        public int Seminarium { get; set; }
        public int Konsultacje { get; set; }
        public int Egzaminy { get; set; }

        public int SumaGodzin =>
            Wyklad + Cwiczenia + Konwersatorium + Laboratorium +
            Warsztaty + Projekt + Seminarium + Konsultacje + Egzaminy;
    }
}
