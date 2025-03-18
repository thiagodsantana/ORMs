using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CsharpORM.EF.Interceptor
{
    public class CustomSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    // Exemplo: adicionar um campo de auditoria
                    //entry.Property("CreatedDate").CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SavingChanges(eventData, result);
        }               
    }
}
