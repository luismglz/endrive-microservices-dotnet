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
    Debug.WriteLine(count);

    if (count == 0)
    {
      Console.WriteLine("No data - will attempt to seed");

      var itemData = await File.ReadAllTextAsync("Data/auctions.json");
      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
      var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

      await DB.SaveAsync(items);
    }
  }
}