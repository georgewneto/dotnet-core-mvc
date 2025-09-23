using Microsoft.EntityFrameworkCore;
using Projeto.RegrasDeNegocio.Models;

namespace Projeto.Infraestrutura.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Estado
            modelBuilder.Entity<Estado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UF).IsRequired().HasMaxLength(2);

                // Relacionamento com Cidade
                entity.HasMany(e => e.Cidades)
                      .WithOne(c => c.Estado)
                      .HasForeignKey(c => c.EstadoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade Cidade
            modelBuilder.Entity<Cidade>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);
                entity.Property(c => c.EstadoId).IsRequired();
            });

            // Dados iniciais para Estados
            modelBuilder.Entity<Estado>().HasData(
                new Estado { Id = 1, Nome = "São Paulo", UF = "SP" },
                new Estado { Id = 2, Nome = "Rio de Janeiro", UF = "RJ" },
                new Estado { Id = 3, Nome = "Minas Gerais", UF = "MG" }
            );

            // Dados iniciais para Cidades
            modelBuilder.Entity<Cidade>().HasData(
                new Cidade { Id = 1, Nome = "São Paulo", EstadoId = 1 },
                new Cidade { Id = 2, Nome = "Campinas", EstadoId = 1 },
                new Cidade { Id = 3, Nome = "Rio de Janeiro", EstadoId = 2 },
                new Cidade { Id = 4, Nome = "Niterói", EstadoId = 2 },
                new Cidade { Id = 5, Nome = "Belo Horizonte", EstadoId = 3 },
                new Cidade { Id = 6, Nome = "Uberlândia", EstadoId = 3 }
            );
        }
    }
}