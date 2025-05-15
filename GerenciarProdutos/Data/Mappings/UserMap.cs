﻿using System.Collections.Generic;
using GerenciarProdutos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GerenciarProdutos.Data.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Tabela
            builder.ToTable("User");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Identity
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            // Propriedades
            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnName("Name")
                .HasColumnType("NVARCHAR")
                .HasMaxLength(80);

            builder.Property(x => x.Bio)
                .IsRequired(false);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnName("Email")
                .HasColumnType("VARCHAR")
                .HasMaxLength(160);

            builder.Property(x => x.Image)
                .IsRequired(false);

            builder.Property(x => x.PasswordHash).IsRequired()
                .HasColumnName("PasswordHash")
                .HasColumnType("VARCHAR")
                .HasMaxLength(255);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasColumnName("Slug")
                .HasColumnType("VARCHAR")
                .HasMaxLength(80);

            // Índices
            builder
                .HasIndex(x => x.Slug, "IX_User_Slug") //cria um index na coluna slug
                .IsUnique();

            // Relacionamentos
            builder
                .HasMany(x => x.Roles) //um User tem varias Roles
                .WithMany(x => x.Users) //uma Role pode estar associada a varios Users
                .UsingEntity<Dictionary<string, object>>( //define uma tabela virtual
                    "UserRole",
                    role => role
                        .HasOne<Role>()
                        .WithMany()
                        .HasForeignKey("RoleId") //chave estrangeira
                        .HasConstraintName("FK_UserRole_RoleId")
                        .OnDelete(DeleteBehavior.Cascade), //deleta em efeito cascata
                    user => user
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRole_UserId")
                        .OnDelete(DeleteBehavior.Cascade));
        }
    }
}