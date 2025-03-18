using CsharpORM.Domain.Classes;
using Microsoft.EntityFrameworkCore;

namespace CsharpORM.Data
{
    /// <summary>
    /// Classe que representa o DbContext do Entity Framework Core.
    /// Usando Code First para definir a estrutura do banco de dados.
    /// </summary>
    public class MeuDbContext(DbContextOptions<MeuDbContext> options) : DbContext(options)
    {
        // Define as tabelas do banco de dados
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }

        /// <summary>
        /// Configuração da estrutura do banco de dados usando Fluent API.
        /// Aqui também é feito o "seeding" de dados iniciais com HasData().
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da relação entre Cliente e Empréstimo
            modelBuilder.Entity<Emprestimo>()
                .HasOne(e => e.Cliente)
                .WithMany(c => c.Emprestimos)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade); // Quando um Cliente for deletado, seus Empréstimos também serão.

            // Seed (dados iniciais) para Clientes
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente { Id = 1, Nome = "João da Silva", Cpf = "123.456.789-00" },
                new Cliente { Id = 2, Nome = "Maria Oliveira", Cpf = "987.654.321-00" }
            );

            // Seed para Empréstimos
            modelBuilder.Entity<Emprestimo>().HasData(
                new Emprestimo { Id = 1, Valor = 1000.00m, Parcelas = 12, TaxaJuros = 2.5m, ClienteId = 1 },
                new Emprestimo { Id = 2, Valor = 5000.00m, Parcelas = 24, TaxaJuros = 3.0m, ClienteId = 1 },
                new Emprestimo { Id = 3, Valor = 2000.00m, Parcelas = 10, TaxaJuros = 1.8m, ClienteId = 2 }
            );
        }
    }
}