using GerenciarProdutos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciarProdutos.Data.Mappings;

public class ProdutoMap : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produto");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();
        
        builder.Property(x => x.Nome)
            .IsRequired()
            .HasColumnName("Nome")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(100);

        builder.Property(x => x.Descricao)
            .IsRequired(false)
            .HasColumnName("Descricao")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(500);

        builder.Property(x => x.Preco)
            .IsRequired()
            .HasColumnName("Preco")
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Estoque)
            .IsRequired()
            .HasColumnName("Estoque")
            .HasColumnType("int");
        
        builder.Property(x => x.DataCriacao)
            .IsRequired(false)
            .HasColumnName("DataCriacao")
            .HasColumnType("datetime");
        
        builder.Property(x => x.DataAtualizaocao)
            .IsRequired(false)
            .HasColumnName("DataAtualizaocao")
            .HasColumnType("datetime");
            


    }
}