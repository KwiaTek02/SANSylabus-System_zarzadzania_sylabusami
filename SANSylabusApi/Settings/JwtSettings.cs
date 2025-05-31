namespace SylabusAPI.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "MyIssuer";
        public string Audience { get; set; } = "MyAudience";
        public string SecretKey { get; set; } = "7Qckl6GAgIZrmCcTs5Jv9JuVadUfEMPPqobA3BKzn0MtmoacVo2CbmEMjg0mZtfj3viGsV1EghNlzEH8TZeNNw==";
        public int ExpiryMinutes { get; set; } = 120;
    }
}


// 7Qckl6GAgIZrmCcTs5Jv9JuVadUfEMPPqobA3BKzn0MtmoacVo2CbmEMjg0mZtfj3viGsV1EghNlzEH8TZeNNw==

// 26c61bb721812709381f4eded743eba5308dd07e50a082e54d3f9fd160a5587b474eb9a9e9f1bdde70288574033b0e57a44519b00d655cd8d847c3b7afd14897