using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using CsharpORM.Dapper.Services;
using CsharpORM.Data;
using CsharpORM.EF.Services.LoadingModes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Perfolizer.Horology;
using System.Data;
using Dapper;

namespace CsharpORM.Benchmark
{
    [Config(typeof(CustomBenchmarkConfig))]
    [MemoryDiagnoser]
    public class ApiBenchmark
    {
        private static readonly string connectionStrig = "Server=127.0.0.1,1433;Database=database;User ID=sa;Password=TnPgk+ZQY378UXmfEpRS04;TrustServerCertificate=True;";
        private readonly IDbConnection dbConnection;
        private readonly MeuDbContext dbContext;

        public ApiBenchmark()
        {
            dbConnection = new SqlConnection(connectionStrig);
            var optionsBuilder = new DbContextOptionsBuilder<MeuDbContext>();
            optionsBuilder.UseSqlServer(connectionStrig);
            dbContext = new MeuDbContext(optionsBuilder.Options);
        }
    
        [Benchmark]
        public Cliente[] DapperClientesComEmprestimos()
        {
           return dbConnection.Query<Cliente>("SELECT c.Id, c.Nome, c.Cpf FROM Clientes c").ToArray();
        }

        [Benchmark]
        public Cliente[] EntityFrameworkClientesComEmprestimos()
        {
            return dbContext.Clientes.AsNoTracking().ToArray();             
        }
    }

    // Configuração personalizada do benchmark
    public class CustomBenchmarkConfig : ManualConfig
    {
        public CustomBenchmarkConfig()
        {
            //AddJob(Job.Default
            //    .WithWarmupCount(3)  // 3 execuções de aquecimento para estabilizar os números
            //    .WithIterationCount(10) // 10 execuções para calcular média e variação
            //    .WithMinIterationTime(TimeInterval.FromMilliseconds(500))); // Evita execuções muito rápidas

            AddExporter(MarkdownExporter.GitHub); // Gera relatório em Markdown
            AddExporter(CsvExporter.Default); // Gera relatório em CSV
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ApiBenchmark>();
        }
    }
}