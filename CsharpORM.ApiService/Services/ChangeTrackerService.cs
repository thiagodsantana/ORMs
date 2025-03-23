using CsharpORM.Data;
using CsharpORM.Domain.Classes;
using Microsoft.EntityFrameworkCore;

namespace CsharpORM.EF.Services
{
    // Serviço que exemplifica o uso do Change Tracker do Entity Framework Core,
    // demonstrando como as alterações em entidades são monitoradas e refletidas no banco de dados.
    public class ChangeTrackerService(MeuDbContext context)
    {
        /// <summary>
        /// Adiciona um novo cliente e exibe os estados da entidade antes e depois da operação.
        /// </summary>
        /// <param name="cliente">Objeto do tipo Cliente a ser adicionado.</param>
        /// <returns>Retorna o cliente adicionado após a confirmação no banco de dados.</returns>
        public async Task<Cliente> AdicionarClienteAsync(Cliente cliente)
        {
            // Exibe o estado inicial da entidade (Detached, pois ela ainda não está sendo rastreada)
            Console.WriteLine($"Estado antes do Add: {context.Entry(cliente).State}"); // Detached

            // Adiciona a entidade ao contexto, tornando seu estado "Added"
            context.Clientes.Add(cliente);
            Console.WriteLine($"Estado após Add: {context.Entry(cliente).State}"); // Added

            // Salva as mudanças no banco, confirmando a inserção, e atualiza o estado para "Unchanged"
            await context.SaveChangesAsync();
            Console.WriteLine($"Estado após SaveChanges: {context.Entry(cliente).State}"); // Unchanged

            return cliente;
        }

        #region Uso de Transactions

        /// <summary>
        /// Adiciona dois clientes em uma única transação implícita.
        /// Todas as operações são agrupadas e salvas juntas.
        /// </summary>
        public void AdicionarClientesTransacaoImplicita()
        {
            // Adiciona duas novas entidades ao contexto
            context.Clientes.Add(new Cliente { Nome = "Antônio João ", Cpf = "95837385059" });
            context.Clientes.Add(new Cliente { Nome = "Josefa Renilde", Cpf = "73653792032" });

            // Salva as mudanças. O EF Core cria uma transação implícita para essa operação.
            context.SaveChanges();
        }

        /// <summary>
        /// Adiciona um cliente e, em seguida, um empréstimo associado a ele, 
        /// utilizando uma transação explícita para garantir a atomicidade das operações.
        /// </summary>
        public void AdicionarClientesTransacaoExplicita()
        {
            // Inicia uma transação explícita no banco de dados
            using var transaction = context.Database.BeginTransaction();
            try
            {
                // Cria e adiciona um novo cliente
                var cliente = new Cliente { Nome = "Carlos Daniel", Cpf = "87612345090" };
                context.Clientes.Add(cliente);
                context.SaveChanges(); // Primeira operação dentro da transação

                // Cria e adiciona um novo empréstimo relacionado ao cliente recém-adicionado
                var emprestimo = new Emprestimo { ClienteId = cliente.Id, Valor = 500, Parcelas = 10, TaxaJuros = 3 };
                context.Emprestimos.Add(emprestimo);
                context.SaveChanges(); // Segunda operação dentro da mesma transação

                // Se todas as operações forem bem-sucedidas, confirma a transação
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Em caso de erro, desfaz todas as operações realizadas dentro da transação
                transaction.Rollback();
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
        #endregion

        /// <summary>
        /// Obtém um cliente pelo ID, modifica seu nome e exibe os estados das alterações.
        /// </summary>
        /// <param name="id">ID do cliente a ser modificado.</param>
        /// <param name="novoNome">Novo nome para o cliente.</param>
        /// <returns>Retorna o cliente atualizado ou null se não encontrado.</returns>
        public async Task<Cliente?> ModificarClienteAsync(int id, string novoNome)
        {
            // Busca o cliente no banco de dados pelo ID
            var cliente = await context.Clientes.FindAsync(id);

            // Se não encontrar, retorna null
            if (cliente == null) return null;

            // Exibe o estado da entidade após a busca (Unchanged)
            Console.WriteLine($"Estado ao buscar: {context.Entry(cliente).State}"); // Unchanged

            // Modifica o nome do cliente
            cliente.Nome = novoNome;
            // O estado da entidade muda para Modified
            Console.WriteLine($"Estado após modificação: {context.Entry(cliente).State}"); // Modified

            // Salva as alterações no banco de dados e o estado retorna a Unchanged
            await context.SaveChangesAsync();
            Console.WriteLine($"Estado após SaveChanges: {context.Entry(cliente).State}"); // Unchanged

            return cliente;
        }

        /// <summary>
        /// Remove um cliente do banco de dados, exibindo os estados de rastreamento antes e após a remoção.
        /// </summary>
        /// <param name="id">ID do cliente a ser removido.</param>
        /// <returns>Retorna true se a remoção foi bem-sucedida, ou false se o cliente não foi encontrado.</returns>
        public async Task<bool> RemoverClienteAsync(int id)
        {
            // Busca o cliente pelo ID
            var cliente = await context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            // Exibe o estado da entidade antes da remoção (Unchanged)
            Console.WriteLine($"Estado antes do Remove: {context.Entry(cliente).State}"); // Unchanged

            // Remove o cliente do contexto, alterando seu estado para Deleted
            context.Clientes.Remove(cliente);
            Console.WriteLine($"Estado após Remove: {context.Entry(cliente).State}"); // Deleted

            // Salva as mudanças, efetivando a remoção no banco
            await context.SaveChangesAsync();
            Console.WriteLine("Cliente removido do banco.");

            return true;
        }

        /// <summary>
        /// Lista todas as entidades que estão sendo rastreadas pelo Change Tracker do contexto.
        /// </summary>
        public void ListarEntidadesRastreadas()
        {
            // Itera sobre todas as entradas rastreadas e exibe o nome da entidade e seu estado atual.
            foreach (var entry in context.ChangeTracker.Entries())
            {
                Console.WriteLine($"Entidade: {entry.Entity.GetType().Name}, Estado: {entry.State}");
            }
        }

        /// <summary>
        /// Reverte todas as alterações realizadas nas entidades rastreadas, restaurando seus valores originais.
        /// </summary>
        public void ReverterAlteracoes()
        {
            // Itera sobre todas as entradas rastreadas no ChangeTracker
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified)
                {
                    // Para entidades modificadas, restaura os valores originais e define o estado como Unchanged
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Added)
                {
                    // Para novas entidades adicionadas, desfaz o rastreamento (state torna-se Detached)
                    entry.State = EntityState.Detached;
                }
            }
            Console.WriteLine("Todas as alterações foram revertidas.");
        }

        /// <summary>
        /// Remove o rastreamento de uma entidade específica (cliente), definindo seu estado para Detached.
        /// </summary>
        /// <param name="cliente">O cliente que deverá ser desanexado do contexto.</param>
        public void DesanexarCliente(Cliente cliente)
        {
            // Define explicitamente o estado da entidade como Detached, removendo-a do Change Tracker.
            context.Entry(cliente).State = EntityState.Detached;
            Console.WriteLine($"Estado após Detach: {context.Entry(cliente).State}"); // Detached
        }
    }
}
