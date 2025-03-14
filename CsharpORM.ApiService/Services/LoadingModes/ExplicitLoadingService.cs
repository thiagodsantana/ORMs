using CsharpORM.Data;
using CsharpORM.Domain.Classes;

namespace CsharpORM.EF.Services.LoadingModes
{
    public class ExplicitLoadingService
    {
        private readonly MeuDbContext _context;

        public ExplicitLoadingService(MeuDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> GetClienteComEmprestimos(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                await _context.Entry(cliente).Collection(c => c.Emprestimos).LoadAsync();
            }
            return cliente;
        }

        public async Task<List<Cliente>> GetClientesComEmprestimos()
        {
            var clientes = _context.Clientes.ToList(); // Busca apenas os clientes


            foreach (var cliente in clientes)
            {
                // Checa se os empréstimos já estão carregados ANTES de acessá-los
                bool estaCarregado = _context.Entry(cliente).Collection(c => c.Emprestimos).IsLoaded;
                Console.WriteLine($"Os empréstimos já estavam carregados? {estaCarregado}");
                await _context.Entry(cliente).Collection(c => c.Emprestimos).LoadAsync(); // Carrega os empréstimos explicitamente
                Console.WriteLine($"Número de empréstimos carregados: {cliente.Emprestimos?.Count}");
            }

            return clientes;
        }
    }

}
