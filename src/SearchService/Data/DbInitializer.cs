using System.Diagnostics;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class DbInitializer
{

  public static async Task InitDb(WebApplication app, WebApplicationBuilder builder)
  {

    var searchServiceSettings = builder.Configuration["Search:ConnectionSettings"];

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

    var count = await DB.CountAsync<Item>();

    using var scope = app.Services.CreateScope();

    var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

    var items = await httpClient.GetItemsForSearchDb();

    Console.WriteLine(items.Count + " returned from the auction service");

    if (items.Count > 0) await DB.SaveAsync(items);
  }
}