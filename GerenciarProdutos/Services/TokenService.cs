using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GerenciarProdutos.Extensions;
using GerenciarProdutos.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GerenciarProdutos.Services;

public class TokenService
{
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler(); //manipula e define o tipo do token
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey); //de string para bytes
        var claims = user.GetClaims(); //guarda o resultado do método que cria as claims (afirmações) do token
        var tokenDescriptor = new SecurityTokenDescriptor //define aa estrutura do token
        {
            Subject = new ClaimsIdentity(claims), //define o usuário e suas características
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), //passamos a chave de assinatura em bytes ja e o tipo que vamos criptografar
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor); //cria o token com as definicoes 
        return tokenHandler.WriteToken(token); //retorna para o metodo como string


    }
}