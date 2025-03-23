using Microsoft.EntityFrameworkCore.Diagnostics;
using Polly;
using System.Data.Common;
using System.Diagnostics;

namespace CsharpORM.EF.Interceptor
{
    /// <summary>
    /// Interceptor personalizado para comandos do Entity Framework.
    /// Implementa logging e política de retry com Polly.
    /// </summary>
    public class CustomDbCommandInterceptor(ILogger<CustomDbCommandInterceptor> logger) : DbCommandInterceptor
    {
        private readonly Stopwatch _stopwatch = new();

        // Cria uma política de retry utilizando Polly
        private readonly AsyncPolicy _retryPolicy = Policy
            .Handle<DbException>() // Define as exceções que dispararão o retry
            .WaitAndRetryAsync(3, // Número máximo de tentativas
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponencial backoff
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning($"Tentativa {retryCount} falhou. Aguardando {timeSpan.TotalSeconds} segundos antes de tentar novamente. Erro: {exception.Message}");
                });

        public override InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData, InterceptionResult<DbCommand> result)
        {
            _stopwatch.Restart();
            return base.CommandCreating(eventData, result);
        }

        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            logger.LogInformation($"Comando SQL Criado: {result.CommandText}");
            return base.CommandCreated(eventData, result);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            _stopwatch.Stop();
            logger.LogInformation($"Executando SQL ({_stopwatch.ElapsedMilliseconds} ms): {command.CommandText}");
            return base.ReaderExecuted(command, eventData, result);
        }

        public override async void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        {
            logger.LogError(eventData.Exception, $"Erro ao executar SQL: {command.CommandText}");

            try
            {
                // Aplicando a política de retry usando Polly
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    logger.LogError("Tentando reexecutar a operação falha...");
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao tentar reexecutar o comando após várias tentativas.");
            }

            base.CommandFailed(command, eventData);
        }
    }
}