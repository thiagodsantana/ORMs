using CsharpORM.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpORM.EF.Services.LoadingModes
{
    /// <summary>
    /// Serviço responsável por demonstrar o carregamento Lazy Loading com o Entity Framework.
    /// </summary>
    public class LazyLoadingService(MeuDbContext context)
    {

        /// <summary>
        /// Busca um cliente pelo ID e acessa seus empréstimos utilizando Lazy Loading.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Cliente com a coleção de empréstimos carregada por Lazy Loading, ou null se não encontrado.</returns>
        public async Task<Cliente?> GetClienteComEmprestimos(int id)
        {
            var cliente = await context.Clientes.FindAsync(id); // Busca o cliente pelo ID

            if (cliente == null)
                return null;

            // Checa se os empréstimos já estão carregados ANTES de acessá-los
            bool estaCarregado = context.Entry(cliente).Collection(c => c.Emprestimos).IsLoaded;
            Console.WriteLine($"Os empréstimos já estavam carregados? {estaCarregado}");

            // Aqui Lazy Loading deve carregar os dados automaticamente ao acessar a propriedade
            var emprestimos = cliente.Emprestimos;
            Console.WriteLine($"Número de empréstimos carregados: {emprestimos?.Count}");

            return cliente;
        }

        /// <summary>
        /// Retorna todos os clientes cadastrados no banco de dados.
        /// </summary>
        /// <returns>Lista de todos os clientes.</returns>
        public async Task<List<Cliente>> GetTodosClientes()
        {
            // Busca todos os clientes sem carregamento explícito de seus empréstimos
            var clientes = await context.Clientes.ToListAsync();

            return clientes;
        }

        /// <summary>
        /// Retorna todos os clientes cadastrados no banco de dados, com empréstimos carregados explicitamente.
        /// </summary>
        /// <returns>Lista de todos os clientes com seus respectivos empréstimos carregados.</returns>
        public async Task<List<Cliente>> GetTodosClientesComEmprestimos()
        {
            // Busca todos os clientes sem carregar os empréstimos
            var clientes = await context.Clientes.ToListAsync();

            // Carrega explicitamente os empréstimos de cada cliente
            foreach (var cliente in clientes)
            {
                // Carrega a coleção de empréstimos explicitamente
                await context.Entry(cliente).Collection(c => c.Emprestimos).LoadAsync();
                var emprestimos = cliente.Emprestimos;
                Console.WriteLine($"Número de empréstimos carregados para o cliente {cliente.Id}: {emprestimos.Count}");
            }

            return clientes;
        }
    }
}
