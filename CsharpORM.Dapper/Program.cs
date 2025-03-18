using CsharpORM.Dapper.Services;
using CsharpORM.Domain.Classes;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

#region
var connectionStrig = "Server=127.0.0.1,1433;Database=database;User ID=sa;Password=TnPgk+ZQY378UXmfEpRS04;TrustServerCertificate=True;";
#endregion

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddProblemDetails();


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.SerializerOptions.WriteIndented = true;
});


builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionStrig));
builder.Services.AddScoped<DapperService>();

// Adiciona Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Clientes e Empréstimos",
        Version = "v1",
        Description = "API para demonstrar o uso do Dapper."
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
}

app.UseRouting();


// Rota GET utilizando Dapper para buscar todos os clientes
app.MapGet("/dapper/clientes", async (DapperService dapperService) =>
{
    var clientes = await dapperService.GetClientesComEmprestimos();
    return Results.Ok(clientes);
});

// Criar um Empréstimo usando Dapper
app.MapPost("/dapper/emprestimos", async (Emprestimo emprestimo, DapperService dapperService) =>
{
    await dapperService.CriarEmprestimo(emprestimo);
    return Results.Created($"/dapper/emprestimos/{emprestimo.Id}", emprestimo);
});

app.Run();