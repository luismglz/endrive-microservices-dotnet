

using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService;
public class CheckAuctionFinished : BackgroundService
{
  private readonly ILogger<CheckAuctionFinished> _logger;
  private readonly IServiceProvider _services;

  public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider services)
  {
    _logger = logger;
    _services = services;
  }
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Starting check for finished auction");

    stoppingToken.Register(() => _logger.LogInformation("==> Auction check is stopping"));

    while (!stoppingToken.IsCancellationRequested)
    {
      await CheckAuctions(stoppingToken);

      await Task.Delay(5000, stoppingToken);
    }
  }

  private async Task CheckAuctions(CancellationToken stoppingToken)
  {
    var finishedAuctions = await DB.Find<Auction>()
      .Match(auction => auction.AuctionEnd <= DateTime.UtcNow)
      .Match(auction => !auction.IsFinished)
      .ExecuteAsync(stoppingToken);

    if (finishedAuctions.Count == 0) return;

    _logger.LogInformation($"==> Found {finishedAuctions.Count} auctions that have completed");

    using var scope = _services.CreateScope();

    var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

    foreach (var auction in finishedAuctions)
    {
      auction.IsFinished = true;

      await auction.SaveAsync(null, stoppingToken);

      var winningBid = await DB.Find<Bid>()
        .Match(bid => bid.AuctionId == auction.ID)
        .Match(bid => bid.BidStatus == BidStatus.Accepted)
        .Sort(bids => bids.Descending(bid => bid.Amount))
        .ExecuteFirstAsync(stoppingToken);

      await endpoint.Publish(new AuctionFinished
      {
        IsItemSold = winningBid != null,
        AuctionId = auction.ID,
        Winner = winningBid?.Bidder,
        Amount = winningBid?.Amount,
        Seller = auction.Seller
      }, stoppingToken);
    }


  }
}
