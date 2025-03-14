using CsharpORM.Data;
using CsharpORM.Domain.Classes;
using CsharpORM.EF.Data.Interceptor;
using CsharpORM.EF.Services;
using CsharpORM.EF.Services.LoadingModes;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddProblemDetails();


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddSingleton<CustomInterceptor>();

/*
 * Configuração do banco de dados com Entity Framework (EF) Core 
 * utilizando Code First e Migrations
 */
builder.AddSqlServerDbContext<MeuDbContext>("database", null, options =>
{
    options
           .UseLazyLoadingProxies() // Habilita Lazy Loading
           .AddInterceptors(builder.Services.BuildServiceProvider() // Adiciona Interceptor personalizado
                                            .GetRequiredService<CustomInterceptor>())
           .LogTo(Console.WriteLine, LogLevel.Information);
    
});

builder.Services.AddScoped<EagerLoadingService>();
builder.Services.AddScoped<ExplicitLoadingService>();
builder.Services.AddScoped<LazyLoadingService>();
builder.Services.AddScoped<ChangeTrackerService>();

// Adiciona Swagger
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

#region Configurando o Swagger
// Ativa Swagger no ambiente de desenvolvimento
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Clientes e Empréstimos v1");
    options.RoutePrefix = "swagger";
});

app.UseSwagger();
app.UseSwaggerUI();

#endregion

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MeuDbContext>();
    context.Database.EnsureCreated();
}

app.UseRouting();

// Eager Loading - Carrega Clientes com Empréstimos na mesma consulta
app.MapGet("eager/clientes", async (EagerLoadingService eagerService) =>
{
    var clientes = await eagerService.GetClientesComEmprestimos().ToListAsync();
    return Results.Ok(clientes);
});

// Explicit Loading - Carrega os Empréstimos de um Cliente sob demanda
app.MapGet("explicit/clientes", async (ExplicitLoadingService explicitService) =>
{
    var cliente = await explicitService.GetClientesComEmprestimos();
    if (cliente is null) return Results.NotFound("Cliente não encontrado");
    return Results.Ok(cliente);
});

// Lazy Loading - Carrega os Empréstimos automaticamente ao acessar a propriedade
app.MapGet("/lazy/clientes/{id}", async (int id, LazyLoadingService lazyService) =>
{
    var cliente = await lazyService.GetClienteComEmprestimos(id);
    if (cliente is null) return Results.NotFound("Cliente não encontrado");
    return Results.Ok(new { Cliente = cliente, Emprestimos = cliente.Emprestimos });
});

// Change Tracker API
app.MapPost("changetracker/clientes", async (Cliente cliente, ChangeTrackerService changeTrackerService) =>
{
    await changeTrackerService.AdicionarClienteAsync(cliente);
    return Results.Created($"/clientes/{cliente.Id}", cliente);
});

app.Run();