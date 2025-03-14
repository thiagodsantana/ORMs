using CsharpORM.Data;
using CsharpORM.Domain.Classes;

namespace CsharpORM.EF.Services.LoadingModes
{
    public class LazyLoadingService(MeuDbContext context)
    {
        public async Task<Cliente?> GetClienteComEmprestimos(int id)
        {
            var cliente = await context.Clientes.FindAsync(id);

            // Checa se os empréstimos já estão carregados ANTES de acessá-los
            bool estaCarregado = context.Entry(cliente).Collection(c => c.Emprestimos).IsLoaded;

            Console.WriteLine($"Os empréstimos já estavam carregados? {estaCarregado}");

            var emprestimos = cliente?.Emprestimos; // Aqui Lazy Loading deve carregar os dados

            Console.WriteLine($"Número de empréstimos carregados: {emprestimos?.Count}");

            return cliente;
        }
    }
}
