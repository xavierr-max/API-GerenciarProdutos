using GerenciarProdutos.Data;
using GerenciarProdutos.Models;
using Microsoft.AspNetCore.Mvc;

namespace GerenciarProdutos.Controllers;

[ApiController]
public class ProdutosController : ControllerBase
{
    [HttpPost ("registro")]
    public async Task<IActionResult> RegistroProduto(
        [FromBody] Produto produto,
        [FromServices] ProdutoDbContext context
    )
    {
        try
        {
            await context.Produtos.AddAsync(produto);
            produto.DataCriacao = DateTime.Now;
            await context.SaveChangesAsync();
            
        }
        catch (Exception)
        {
            return BadRequest();
        }
        return Ok();
    }
}