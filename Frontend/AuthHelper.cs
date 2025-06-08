namespace Frontend
{
    public static class JwtHelper
    {
        public static bool IsTokenExpired(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return true;

            var payload = System.Text.Json.JsonDocument.Parse(
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(AddPadding(parts[1]))));

            var expUnix = payload.RootElement.GetProperty("exp").GetInt64();
            var expTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            return DateTime.UtcNow > expTime;
        }

        private static string AddPadding(string base64)
        {
            return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        }
    }

}
