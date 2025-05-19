using System.Text.RegularExpressions;
using Blog.ViewModels.Accounts;
using GerenciarProdutos.Data;
using GerenciarProdutos.Extensions;
using GerenciarProdutos.Models;
using GerenciarProdutos.Services;
using GerenciarProdutos.ViewModels;
using GerenciarProdutos.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
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
        [FromServices] ProdutoDbContext context, //acesso ao banco de dados
        [FromServices] EmailService emailService)
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

            emailService.Send(user.Name, user.Email, "Bem vindo ao blog!", $"Sua senha é {password}");
            
            return Ok(new ResultViewModel<dynamic>(new 
                {user = user.Email, password})); //retorna um status 200 para tela e os atributos de user
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

    [Authorize]
    [HttpPost("conta/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] ProdutoDbContext context)
    {
        var filename = $"{Guid.NewGuid().ToString()}.jpg"; //gera um guid para nao repetir o nome do arquivo onde vai ser salvo
        var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, ""); 
        //método que remove e poe a imagem 
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{filename}", bytes); 
            //passa o local de save da imagem e passa qual é a imagem
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }

        var user = await context
            .Users
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        
        if(user == null)
            return NotFound(new ResultViewModel<Produto>("Usuário não encontrado"));

        user.Image = $"https://localhost:0000/images/{filename}";
        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }

        return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!", null));
        
    }
    
}