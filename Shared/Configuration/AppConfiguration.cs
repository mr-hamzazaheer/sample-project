namespace Shared
{
    public class AppSettings
    {
        public string[] CorsUrl { get; set; }
        public string DefaultCulture { get; set; }
        public AuthCredential AuthCredential { get; set; }
        public Jwt Jwt { get; set; }
    }
    public class Jwt
    {
        public int TokenExpiryMinutes { get; set; }
        public string Key { get; set; }
        public string Issuer { get; set; }
    }
    public class AuthCredential
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
