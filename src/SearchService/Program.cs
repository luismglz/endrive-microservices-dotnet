using MongoDB.Driver;
using MongoDB.Entities;
using SearchService;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try
{
  await DbInitializer.InitDb(app, builder);
}
catch (Exception e)
{
  Console.WriteLine(e);
}

app.Run();
