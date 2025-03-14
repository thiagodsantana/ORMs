using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using CsharpORM.ApiService.Classes;
using CsharpORM.ApiService.Data;
using CsharpORM.ApiService.Services.Dapper;
using CsharpORM.ApiService.Services.EF.LoadingModes;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o contexto do banco de dados e serviços
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer("database"));
builder.Services.AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory("database"));
builder.Services.AddScoped<DapperService>();
builder.Services.AddScoped<EagerLoadingService>();

var app = builder.Build();

// Configura Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/benchmark", () => Results.Ok(BenchmarkRunner.Run<DatabaseBenchmark>(
    ManualConfig.Create(DefaultConfig.Instance)
        .WithOptions(ConfigOptions.DisableOptimizationsValidator)))).WithName("RunBenchmark");

app.Run();

public class DatabaseBenchmark
{
    private readonly DapperService _dapperService;
    private readonly EagerLoadingService _eagerLoadingService;

    public DatabaseBenchmark()
    {
        var services = new ServiceCollection()
            .AddDbContext<MeuDbContext>(options => options.UseSqlServer("database"))
            .AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory("database"))
            .AddScoped<DapperService>()
            .AddScoped<EagerLoadingService>()
            .BuildServiceProvider();

        _dapperService = services.GetRequiredService<DapperService>();
        _eagerLoadingService = services.GetRequiredService<EagerLoadingService>();
    }

    [Benchmark]
    public async Task<List<Cliente>> EF_GetClientes() => await _eagerLoadingService.GetClientesComEmprestimos().ToListAsync();

    [Benchmark]
    public async Task<List<Cliente>> Dapper_GetClientes() => (await _dapperService.GetClientes()).ToList();
}
