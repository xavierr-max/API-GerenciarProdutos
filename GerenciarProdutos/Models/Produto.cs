﻿namespace GerenciarProdutos.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizaocao { get; set; }
}