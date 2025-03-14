var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql", port: 1433)
                 .WithLifetime(ContainerLifetime.Persistent) //Recomendável utilizar uma vida útil persistente para evitar reinicializações desnecessárias.
                 .WithDataVolume();
// usado para armazenar permanentemente os dados de SQL Server
// fora do ciclo de vida de seu contêiner.

var db = sql.AddDatabase("database");

var apiEF = builder.AddProject<Projects.CsharpORM_EF>("APIEntityFramework")
       .WithReference(db)
       .WaitFor(db);

var apiDapper = builder.AddProject<Projects.CsharpORM_Dapper>("APIDapper")
       .WithReference(db)
       .WaitFor(db);

builder.AddProject<Projects.CsharpORM_Benchmark>("Benchmark")
    .WithReference(apiDapper)
    .WithReference(apiEF)
    .WaitFor(apiDapper)
    .WaitFor(apiEF);



builder.Build().Run();
