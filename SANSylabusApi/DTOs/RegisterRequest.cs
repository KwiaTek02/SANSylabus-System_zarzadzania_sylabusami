namespace SylabusAPI.DTOs
{
    public class RegisterRequest
    {
        public string ImieNazwisko { get; set; } = default!;
        public string? Tytul { get; set; }
        public string Login { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string TypKonta { get; set; } = "gosc";
    }
}
