﻿namespace GerenciarProdutos;

public static class Configuration
{
    public static string? JwtKey { get; set; } = "CALMA CALABRESO";
    public static SmtpConfiguration Smtp = new();
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string Username { get; set; }
        public string Password { get; set; }
    }
}