namespace SylabusAPI.Settings
{
    // Klasa zawierająca ustawienia konfiguracyjne dla uwierzytelniania JWT (JSON Web Token)
    public class JwtSettings
    {
        // Wystawca tokena (np. nazwa aplikacji lub systemu)
        public string Issuer { get; set; } = "MyIssuer";

        // Odbiorca tokena (np. klient lub usługa, która ma prawo używać tokena)
        public string Audience { get; set; } = "MyAudience";

        // Tajny klucz używany do podpisywania tokenów JWT
        public string SecretKey { get; set; } = "7Qckl6GAgIZrmCcTs5Jv9JuVadUfEMPPqobA3BKzn0MtmoacVo2CbmEMjg0mZtfj3viGsV1EghNlzEH8TZeNNw==";

        // Czas ważności tokena (w minutach)
        public int ExpiryMinutes { get; set; } = 120;
    }
}



// 7Qckl6GAgIZrmCcTs5Jv9JuVadUfEMPPqobA3BKzn0MtmoacVo2CbmEMjg0mZtfj3viGsV1EghNlzEH8TZeNNw==

// 26c61bb721812709381f4eded743eba5308dd07e50a082e54d3f9fd160a5587b474eb9a9e9f1bdde70288574033b0e57a44519b00d655cd8d847c3b7afd14897