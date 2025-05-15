using GerenciarProdutos.Data.Mappings;
using GerenciarProdutos.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciarProdutos.Data;

public class ProdutoDbContext : DbContext
{
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=GerenciarProdutosDb;User ID=sa;Password=1q2w3e4r@#$;Trusted_Connection=False; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProdutoMap());
        modelBuilder.ApplyConfiguration(new UserMap());
    }
}