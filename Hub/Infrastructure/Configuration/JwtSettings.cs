namespace Infrastructure.Configuration;

internal class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int AccessTokenExpirationInMinutes { get; set; }
}
