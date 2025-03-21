using CsharpORM.Data;
using CsharpORM.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using System;

namespace CsharpORM.EF.Services
{
    public class ChangeTrackerService(MeuDbContext context)
    {
        // Adiciona um novo cliente e exibe o estado
        public async Task<Cliente> AdicionarClienteAsync(Cliente cliente)
        {
            Console.WriteLine($"Estado antes do Add: {context.Entry(cliente).State}"); // Detached

            context.Clientes.Add(cliente);
            Console.WriteLine($"Estado após Add: {context.Entry(cliente).State}"); // Added

            await context.SaveChangesAsync();
            Console.WriteLine($"Estado após SaveChanges: {context.Entry(cliente).State}"); // Unchanged

            return cliente;
        }

        #region Uso de Transactions
        public void AdicionarClientesTransacaoImplicita()
        {
            // Adiciona duas entidades
            context.Clientes.Add(new Cliente { Nome = "Antônio João ", Cpf = "95837385059" });
            context.Clientes.Add(new Cliente { Nome = "Josefa Renilde", Cpf = "73653792032" });

            // Efetua todas as operações dentro de uma única transação
            context.SaveChanges();

        }

        public void AdicionarClientesTransacaoExplicita()
        {
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var cliente = new Cliente { Nome = "Carlos Daniel", Cpf = "87612345090" };
                context.Clientes.Add(cliente);
                context.SaveChanges(); // Primeira operação dentro da transação

                var emprestimo = new Emprestimo { ClienteId = cliente.Id, Valor = 500, Parcelas = 10, TaxaJuros = 3 };
                context.Emprestimos.Add(emprestimo);
                context.SaveChanges(); // Segunda operação dentro da mesma transação

                transaction.Commit(); // Se tudo der certo, confirma a transação
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // Se ocorrer erro, desfaz tudo
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
        #endregion

        // Obtém um cliente, modifica seu nome e mostra estados
        public async Task<Cliente?> ModificarClienteAsync(int id, string novoNome)
        {
            var cliente = await context.Clientes.FindAsync(id);

            if (cliente == null) return null;

            Console.WriteLine($"Estado ao buscar: {context.Entry(cliente).State}"); // Unchanged

            cliente.Nome = novoNome;
            Console.WriteLine($"Estado após modificação: {context.Entry(cliente).State}"); // Modified

            await context.SaveChangesAsync();
            Console.WriteLine($"Estado após SaveChanges: {context.Entry(cliente).State}"); // Unchanged

            return cliente;
        }

        // Obtém um cliente e remove do banco
        public async Task<bool> RemoverClienteAsync(int id)
        {
            var cliente = await context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            Console.WriteLine($"Estado antes do Remove: {context.Entry(cliente).State}"); // Unchanged

            context.Clientes.Remove(cliente);
            Console.WriteLine($"Estado após Remove: {context.Entry(cliente).State}"); // Deleted

            await context.SaveChangesAsync();
            Console.WriteLine("Cliente removido do banco.");

            return true;
        }

        // Lista todas as entidades rastreadas pelo Change Tracker
        public void ListarEntidadesRastreadas()
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                Console.WriteLine($"Entidade: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }
        }

        // Desfazer todas as mudanças antes de salvar
        public void ReverterAlteracoes()
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.State = EntityState.Detached;
                }
            }
            Console.WriteLine("Todas as alterações foram revertidas.");
        }

        // Remover entidade do rastreamento (Detach)
        public void DesanexarCliente(Cliente cliente)
        {
            context.Entry(cliente).State = EntityState.Detached;
            Console.WriteLine($"Estado após Detach: {context.Entry(cliente).State}"); // Detached
        }
    }
}
