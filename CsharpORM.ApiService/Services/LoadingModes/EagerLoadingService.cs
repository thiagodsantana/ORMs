using CsharpORM.Data;
using Microsoft.EntityFrameworkCore;

namespace CsharpORM.EF.Services.LoadingModes
{
    // Serviço responsável por demonstrar o carregamento (Eager Loading)
    // com o Entity Framework, que carrega entidades relacionadas
    // junto com a consulta principal.
    public class EagerLoadingService(MeuDbContext context)
    {

        /// <summary>
        /// Retorna uma consulta (IQueryable) que busca os clientes juntamente com seus empréstimos.
        /// Utiliza o método Include para realizar o carregamento (Eager Loading) dos empréstimos.
        /// </summary>
        /// <returns>IQueryable contendo os clientes com suas respectivas coleções de empréstimos.</returns>
        public async Task<List<Cliente>> GetClientesComEmprestimos()
        {
            // O método Include indica ao EF para carregar a propriedade de navegação "Emprestimos"
            // associada a cada Cliente, realizando o carregamento Eager Loading.
            return await context.Clientes.Include(c => c.Emprestimos).ToListAsync();
        }
    }
}
