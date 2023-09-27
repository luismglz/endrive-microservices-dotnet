using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

var searchServiceSettings = builder.Configuration["Search:ConnectionSettings"];

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

await DB.InitAsync(
  "searchDB",
  MongoClientSettings.FromConnectionString(searchServiceSettings)
  );


//add index to those Item class props for mongo

await DB.Index<Item>()
  .Key(item => item.Make, KeyType.Text)
  .Key(item => item.Model, KeyType.Text)
  .Key(item => item.Color, KeyType.Text)
.CreateAsync();

app.Run();
