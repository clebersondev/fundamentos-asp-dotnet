namespace BlogEF3;

public static class Configuration
{
    public static string JwtKey = "5X8MVCSxnSHfLUsV8B6ODiKq01g86MiH";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "curso_api_2s;xFEFTIij0]8Cs2i%V";
    public static SmtpConfiguration Smtp = new();
}

public class SmtpConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; } = 25;
    public string UserName { get; set; }
    public string Password { get; set; }
}