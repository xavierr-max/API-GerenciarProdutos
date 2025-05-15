using GerenciarProdutos.Data;
using GerenciarProdutos.Extensions;
using GerenciarProdutos.Models;
using GerenciarProdutos.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciarProdutos.Controllers;

[ApiController]
[Authorize (Roles = "Admin")]
public class ProdutosController : ControllerBase
{
    [HttpPost ("registro")]
    public async Task<IActionResult> RegistroProduto(
        [FromBody] EditorProdutoViewModel model,
        [FromServices] ProdutoDbContext context)
    {   
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Produto>(ModelState.GetErrors()));
        
        var produto = new Produto
        {
            Nome = model.Nome,
            Preco = model.Preco,
            Estoque = model.Estoque,
            DataCriacao = DateTime.Now
        };
        
        try
        {
            await context.Produtos.AddAsync(produto);
            await context.SaveChangesAsync();
            
            return Ok(new ResultViewModel<dynamic>(new ResultViewModel<dynamic>
                ($"O produto {produto.Nome} foi registrado no sistema!", null)));
        }
        catch (Exception)
        {
            return BadRequest(new ResultViewModel<string>("[ExA01]não foi possível adicionar o registro"));
        }
        
    }
}