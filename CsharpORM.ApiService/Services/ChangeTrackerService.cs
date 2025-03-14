using CsharpORM.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpORM.EF.Services
{
    public class ChangeTrackerService(MeuDbContext context)
    {
        // ✅ Adiciona um novo cliente e exibe o estado
        public async Task<Cliente> AdicionarClienteAsync(Cliente cliente)
        {
            Console.WriteLine($"Estado antes do Add: {context.Entry(cliente).State}"); // Detached

            context.Clientes.Add(cliente);
            Console.WriteLine($"Estado após Add: {context.Entry(cliente).State}"); // Added

            await context.SaveChangesAsync();
            Console.WriteLine($"Estado após SaveChanges: {context.Entry(cliente).State}"); // Unchanged

            return cliente;
        }

        // ✅ Obtém um cliente, modifica seu nome e mostra estados
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

        // ✅ Obtém um cliente e remove do banco
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

        // ✅ Lista todas as entidades rastreadas pelo Change Tracker
        public void ListarEntidadesRastreadas()
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                Console.WriteLine($"Entidade: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }
        }

        // ✅ Desfazer todas as mudanças antes de salvar
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

        // ✅ Remover entidade do rastreamento (Detach)
        public void DesanexarCliente(Cliente cliente)
        {
            context.Entry(cliente).State = EntityState.Detached;
            Console.WriteLine($"Estado após Detach: {context.Entry(cliente).State}"); // Detached
        }
    }
}
