using System.ComponentModel.DataAnnotations;

namespace GerenciarProdutos.ViewModels;

public class EditorProdutoViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 100 caracteres")]
    public string Nome { get; set; }

    [StringLength(500, ErrorMessage = "A descrição deve conter no máximo 500 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O preço é obrigatório")]
    [Range(0.01, 999999.99, ErrorMessage = "O preço deve estar entre 0,01 e 999.999,99")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "O estoque é obrigatório")]
    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo")]
    public int Estoque { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? DataCriacao { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? DataAtualizacao { get; set; }
}