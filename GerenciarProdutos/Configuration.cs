namespace GerenciarProdutos;

public static class Configuration
{
    public static string? JwtKey { get; set; } = "ZXh0cmVtZWx5LXNlY3VyZS1rZXktZm9yLWp3dC1hdXRoZW50aWNhdGlvbi1taW5pbXVtLTMyLWNoYXJhY3RlcnM=";
    public static SmtpConfiguration Smtp = new();
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string Username { get; set; }
        public string Password { get; set; }
    }
}