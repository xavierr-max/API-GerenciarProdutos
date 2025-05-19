using GerenciarProdutos.Data;
using GerenciarProdutos.Extensions;
using GerenciarProdutos.Models;
using GerenciarProdutos.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GerenciarProdutos.Controllers;

[ApiController]

public class ProdutosController : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("registro")]
    public async Task<IActionResult> RegistroProduto(
        [FromBody] EditorProdutoViewModel model,
        [FromServices] ProdutoDbContext context
        )
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
    

        [HttpGet("loja")]
        public async Task<IActionResult> Get( //método get
            [FromServices] ProdutoDbContext context, //acesso ao banco
            [FromServices] IMemoryCache cache, //guarda na memoria para evitar hit no banco
            [FromQuery] int page = 0, //paginas com elementos
            [FromQuery] int pageSize = 25) //elementos da pagina
        {
            var prods = cache.GetOrCreate("ProdutosCache", entry => //atualiza ou cria um novo cache
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1); //tempo de duracao para atualizar
                return GetProdutos(context); //retorna o método que da hit no banco e salva no cache
            });
            try
            {
                var count = await context.Produtos.AsNoTracking().CountAsync();
                var produtos = await context //query inicial de acesso ao banco
                    .Produtos //acesso a tabela Produtos
                    .AsNoTracking() //sem rastreamento
                    .Select(x =>
                        new LojaViewModel //faz um select no banco pegando informacoes personalizadas para o ViewModel
                        {
                            Nome = x.Nome,
                            Preco = x.Preco,
                            Estoque = x.Estoque
                        })
                    .Skip(page *
                          pageSize) //determina quantos registros devem ser ignorados para chegar ao início da página desejada.
                    .Take(pageSize) //define quantos registros serão incluídos no resultado.
                    .ToListAsync(); //passa pelo banco, nao se restringindo a memoria
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    produtos
                })); //retorna um status ok com as informacoes paginadas
            }
            catch (Exception e)
            {
                return StatusCode(501, new ResultViewModel<string>(e.Message));
            }
        }
        private List<Produto> GetProdutos(ProdutoDbContext context) //método que passa pelo banco para salvar no cache
        {
            return context.Produtos.ToList();
        }

    }
