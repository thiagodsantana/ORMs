using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace CsharpORM.EF.Data.Interceptor
{
    public class CustomInterceptor(ILogger<CustomInterceptor> logger) : DbCommandInterceptor
    {
        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            logger.LogInformation($"Comando SQL Executado: {result.CommandText}");
            return base.CommandCreated(eventData, result);
        }
    }
}