using CsharpORM.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpORM.EF.Services.LoadingModes
{
    /// <summary>
    /// Serviço responsável por demonstrar o carregamento explícito (Explicit Loading)
    /// com o Entity Framework.
    /// </summary>
    public class ExplicitLoadingService(MeuDbContext context)
    {
        /// <summary>
        /// Busca um cliente pelo ID e carrega explicitamente seus empréstimos.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Cliente com a coleção de empréstimos carregada, ou null se não encontrado.</returns>
        public async Task<Cliente?> GetClienteComEmprestimos(int id)
        {
            var cliente = await context.Clientes.FindAsync(id); // Busca o cliente pelo ID no banco de dados
            if (cliente != null)
            {
                // Carrega explicitamente a coleção de empréstimos do cliente
                await context.Entry(cliente).Collection(c => c.Emprestimos).LoadAsync();
            }
            return cliente; // Retorna o cliente com os empréstimos carregados
        }

        /// <summary>
        /// Busca todos os clientes e carrega explicitamente seus empréstimos.
        /// </summary>
        /// <returns>Lista de clientes com seus respectivos empréstimos carregados.</returns>
        public async Task<List<Cliente>> GetClientesComEmprestimos()
        {
            var clientes = await context.Clientes.ToListAsync(); // Busca a lista de clientes do banco de dados

            foreach (var cliente in clientes)
            {
                // Verifica se os empréstimos já foram carregados antes de acessá-los
                bool estaCarregado = context.Entry(cliente).Collection(c => c.Emprestimos).IsLoaded;
                Console.WriteLine($"Os empréstimos já estavam carregados? {estaCarregado}");

                // Se os empréstimos não estiverem carregados, realiza o carregamento explícito
                if (!estaCarregado)
                {
                    await context.Entry(cliente).Collection(c => c.Emprestimos).LoadAsync();
                }

                Console.WriteLine($"Número de empréstimos carregados: {cliente.Emprestimos?.Count}");
            }

            return clientes; // Retorna a lista de clientes com os empréstimos carregados
        }
    }
}