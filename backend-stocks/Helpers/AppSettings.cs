namespace WebApi.Helpers;

public class AppSettings
{
    public string Secret { get; set; } = string.Empty!;

    public int RefreshTokenTTL { get; set; }
}
