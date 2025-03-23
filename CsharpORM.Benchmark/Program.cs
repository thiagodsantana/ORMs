using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

[Config(typeof(CustomBenchmarkConfig))]
[MemoryDiagnoser]
[RankColumn]
public class ApiBenchmark
{
    private static readonly HttpClient httpClient = new();

    [Params(10)] // Testar com diferentes quantidades de registros
    public int RecordCount;

    [Benchmark]
    public async Task<string> DapperClientesComEmprestimos()
    {
        return await httpClient.GetStringAsync($"https://localhost:7015/dapper/clientes?count={RecordCount}");
    }

    [Benchmark]
    public async Task<string> EntityFrameworkClientesComEmprestimos()
    {
        return await httpClient.GetStringAsync($"https://localhost:7512/eager/clientes?count={RecordCount}");
    }
}

// Configuração personalizada do benchmark
public class CustomBenchmarkConfig : ManualConfig
{
    public CustomBenchmarkConfig()
    {
        AddJob(Job.Default
            .WithWarmupCount(3)  // 3 execuções de aquecimento para estabilizar os números
            .WithIterationCount(10) // 10 execuções para calcular média e variação
            .WithMinIterationTime(TimeInterval.FromMilliseconds(500))); // Evita execuções muito rápidas

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
