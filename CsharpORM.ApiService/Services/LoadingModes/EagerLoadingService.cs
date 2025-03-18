using CsharpORM.Data;
using System.Data.Entity;

namespace CsharpORM.EF.Services.LoadingModes
{
    public class EagerLoadingService(MeuDbContext context)
    {
        public IQueryable<Cliente> GetClientesComEmprestimos()
        {
            return context.Clientes.Include(c => c.Emprestimos);
        }
    }

}
