namespace Application
{
    public class JwtSettings
    {
        public virtual string SecretKey { get; set; } = "MySecretKey123456789asdasdasdas0";
        public virtual string? Audience { get; set; } = "https://localhost:7234";
        public virtual string? Issuer { get; set; } = "https://localhost:7234";
        public virtual int TokenValidityMinutes { get; set; } = 50;
        public virtual double RefreshTokenValidityDays { get; set; } = 7;
    }
}
