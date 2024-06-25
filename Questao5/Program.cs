using MediatR;
using Questao5.Infrastructure.Sqlite;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var databaseConfig = new DatabaseConfig { Name = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite") };
builder.Services.AddSingleton(databaseConfig);
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

builder.Services.AddTransient<IDbConnection>((sp) => new SqliteConnection(databaseConfig.Name));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var dbBootstrap = app.Services.GetService<IDatabaseBootstrap>();
dbBootstrap!.Setup();

app.Run();