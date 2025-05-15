using System.Security.Claims;
using GerenciarProdutos.Models;

namespace GerenciarProdutos.Extensions;

public static class ClaimsExtension
{
    public static IEnumerable<Claim> GetClaims(this User user) //personaliza o token com as claims
    {
        var claims = new List<Claim> //cria uma lista de claims
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
            //primero passamos qual tipo da claim que vamos incluir ao token, e depois da virgula é passado o atributo do user de acordo com a claim
        };
        foreach (var role in user.Roles) //um foreach de todos os roles ligados ao user que esta sendo passado
            claims.Add(new Claim(ClaimTypes.Role, role.Name)); //adiciona mesma lista de claims os roles dele
        
        return claims; //retorna a lista de claims com os atributos e roles
    }
}