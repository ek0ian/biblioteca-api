namespace BibliotecaApi.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public int ExpirationHours { get; set; } = 8;
    public string Issuer { get; set; } = "BibliotecaApi";
    public string Audience { get; set; } = "BibliotecaApi";
}
