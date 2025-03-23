using CsharpORM.Data;
using CsharpORM.EF.Interceptor;
using CsharpORM.EF.Services;
using CsharpORM.EF.Services.LoadingModes;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona configurações padrões e integrações do .NET Aspire
builder.AddServiceDefaults();

// Adiciona serviços ao contêiner
builder.Services.AddProblemDetails();

// Configuração do JSON para preservar referências circulares e formatar saída
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.SerializerOptions.WriteIndented = true;
});

// Registra o interceptor de comandos personalizados do Entity Framework
builder.Services.AddSingleton<CustomDbCommandInterceptor>();

/*
 * Configuração do banco de dados com Entity Framework (EF) Core 
 * utilizando Code First e Migrations
 */
builder.Services.AddDbContext<MeuDbContext>((sp, options) =>
{
    // Obtém o interceptor do container DI
    var customDbCommandInterceptor = sp.GetRequiredService<CustomDbCommandInterceptor>();

    options
        .UseSqlServer(builder.Configuration.GetConnectionString("database")) // Configuração correta do SQL Server
        .UseLazyLoadingProxies() // Habilita Lazy Loading
        .AddInterceptors(customDbCommandInterceptor); // Adiciona Interceptor personalizado
});

// Registra os serviços de carregamento de dados
builder.Services.AddScoped<EagerLoadingService>();
builder.Services.AddScoped<ExplicitLoadingService>();
builder.Services.AddScoped<LazyLoadingService>();
builder.Services.AddScoped<ChangeTrackerService>();

// Adiciona suporte ao Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Clientes e Empréstimos",
        Version = "v1",
        Description = "API para demonstrar diferentes tipos de carregamento de dados (Eager, Explicit, Lazy) utilizando Entity Framework."
    });
});

var app = builder.Build();

#region Configuração do Swagger
// Ativa Swagger no ambiente de desenvolvimento
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Clientes e Empréstimos v1");
    options.RoutePrefix = "swagger";
});
#endregion

// Verifica se está no ambiente de desenvolvimento e inicializa o banco de dados
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MeuDbContext>();
    context.Database.EnsureCreated();
}

app.UseRouting();

// Endpoint para Eager Loading - Carrega Clientes com Empréstimos na mesma consulta
app.MapGet("eager/clientes", async (EagerLoadingService eagerService) =>
{
    var clientes = await eagerService.GetClientesComEmprestimos();
    return Results.Ok(clientes);
});

// Endpoint para Explicit Loading - Carrega os Empréstimos de um Cliente sob demanda
app.MapGet("explicit/clientes", async (ExplicitLoadingService explicitService) =>
{
    var cliente = await explicitService.GetClientesComEmprestimos();
    return Results.Ok(cliente);
});

// Endpoint para Lazy Loading - Carrega os Empréstimos automaticamente ao acessar a propriedade
app.MapGet("/lazy/clientes", async (LazyLoadingService lazyService) =>
{
    var clientes = await lazyService.GetTodosClientesComEmprestimos();
    return Results.Ok(clientes);
});

// Endpoint para Change Tracker API - Adiciona um cliente ao banco de dados
app.MapPost("changetracker/clientes", async (Cliente cliente, ChangeTrackerService changeTrackerService) =>
{
    await changeTrackerService.AdicionarClienteAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", cliente);
});

// Inicia a aplicação
app.Run();
