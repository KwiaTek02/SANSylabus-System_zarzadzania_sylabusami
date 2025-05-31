namespace Frontend.Shared
{
    public class PrzedmiotDto
    {
        public int Id { get; set; }
        public string Nazwa { get; set; } = default!;
        public string? Osrodek { get; set; }
        public byte? Semestr { get; set; }
        public string? Stopien { get; set; }
        public string? Kierunek { get; set; }
        public int? SumaGodzinCalosciowe { get; set; }
    }
}
