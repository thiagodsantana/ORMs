using CsharpORM.Domain.Classes;
using Microsoft.EntityFrameworkCore;

namespace CsharpORM.Data
{
    // Aplicação de Code First e Migrations no DbContext
    public class MeuDbContext(DbContextOptions<MeuDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Dados fictícios para Clientes
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente { Id = 1, Nome = "João da Silva", Cpf = "123.456.789-00" },
                new Cliente { Id = 2, Nome = "Maria Oliveira", Cpf = "987.654.321-00" }
            );

            // Dados fictícios para Empréstimos
            modelBuilder.Entity<Emprestimo>().HasData(
                new Emprestimo { Id = 1, Valor = 1000.00m, Parcelas = 12, TaxaJuros = 2.5m, ClienteId = 1 },
                new Emprestimo { Id = 2, Valor = 5000.00m, Parcelas = 24, TaxaJuros = 3.0m, ClienteId = 1 },
                new Emprestimo { Id = 3, Valor = 2000.00m, Parcelas = 10, TaxaJuros = 1.8m, ClienteId = 2 }
            );
        }
    }
}
