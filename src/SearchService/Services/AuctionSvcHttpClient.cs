using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionSvcHttpClient
{

  private readonly HttpClient _httpClient;
  private readonly IConfiguration _config;

  public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
  {
    _httpClient = httpClient;
    _config = config;
  }

  public async Task<List<Item>> GetItemsForSearchDb()
  {

    var auctionServiceUrl = _config["Search:AuctionServiceUrl"];

    var lastUpdated = await DB.Find<Item, string>()
    .Sort(item => item.Descending(item => item.UpdatedAt))
    .Project(item => item.UpdatedAt.ToString())
    .ExecuteFirstAsync();

    return await _httpClient.GetFromJsonAsync<List<Item>>(auctionServiceUrl + "/api/auctions?date=" + lastUpdated);
  }

}
