using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace Consumers;
public class AuctionUpdatedConsumer : IConsumer
{

  private readonly IMapper _mapper;

  public AuctionUpdatedConsumer(IMapper mapper)
  {
    _mapper = mapper;
  }

  public async Task Consume(ConsumeContext<AuctionUpdated> context)
  {
    Console.WriteLine("--> Consuming auction updated: " + context.Message.Id);

    var itemToUpdateMapped = _mapper.Map<Item>(context.Message);

    var result = await DB.Update<Item>()
    .Match(auction => auction.ID == context.Message.Id)
    .ModifyOnly(item => new
    {
      item.Color,
      item.Make,
      item.Model,
      item.Year,
      item.Mileage
    }, itemToUpdateMapped)
    .ExecuteAsync();

    if (!result.IsAcknowledged)
      throw new MessageException(typeof(AuctionUpdated), "There was a problem updating search term item");

  }

}
