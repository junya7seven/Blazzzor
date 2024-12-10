namespace Application
{
    public class JwtSettings
    {
        public virtual string SecretKey { get; set; }
        public virtual string? Audience { get; set; } = null;
        public virtual string? Issuer { get; set; } = null;
        public virtual int TokenValidityMinutes { get; set; } = 5;
        public virtual double RefreshTokenValidityDays { get; set; } = 7;
    }
}
