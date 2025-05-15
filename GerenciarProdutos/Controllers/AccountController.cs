using Blog.ViewModels.Accounts;
using GerenciarProdutos.Data;
using GerenciarProdutos.Extensions;
using GerenciarProdutos.Models;
using GerenciarProdutos.Services;
using GerenciarProdutos.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace GerenciarProdutos.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("conta")]
    public async Task<IActionResult> Post( //task para realizar o cadastro pelo nome e email e fornece uma senhahash
        [FromBody] RegisterViewModel model, //recebe as informacoes da requisicao e envia para a classe RegisterViewModel
        [FromServices] ProdutoDbContext context) //acesso ao banco de dados
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors())); 
        //retorna um erro caso a estrutura da requisicao nao corresponder a JSON

        var user = new User() //objeto instaciado de Models recebendo as info da requisicao, e depois passa para o banco
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-"),
        };

        var password = PasswordGenerator.Generate(25); //gera uma senha 
        user.PasswordHash = PasswordHasher.Hash(password);// hashea a senha

        try
        {
            await context.Users.AddAsync(user); //tenta enviar as info na tabela Users
            await context.SaveChangesAsync(); //salva as alteracoes no banco
            return Ok(new ResultViewModel<dynamic>(new {user = user.Email, password})); //retorna um status 200 para tela e os atributos de user
        }
        catch (DbUpdateException)//caso ocorra algum erro...
        {
            return StatusCode(400, new ResultViewModel<string>("05X99 - E-mail já cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna"));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] TokenService tokenService, //usa o servico de token criado
        [FromServices] ProdutoDbContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        
        var user = await context.Users
            .AsNoTracking() //sem rastreamento
            .Include(x => x.Roles) //incluir na busca os roles relacionados ao user
            .FirstOrDefaultAsync(x => x.Email == model.Email); //filtra por email iguais

        if (user == null || !PasswordHasher.Verify(user.PasswordHash, model.Password)) //sendo o user null ou as senhashash nao correponder, retorna um erro
            return StatusCode(401, new ResultViewModel<string>($"Senha incorreta"));

        try
        {
            var token = tokenService.GenerateToken(user); //gera uma chave com base no user filtrado
            return Ok(new ResultViewModel<string>(token, null)); //retorna o token na tela da requisicao
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna"));
        }
    }
    
}