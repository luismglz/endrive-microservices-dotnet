using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
  public MappingProfiles()
  {
    CreateMap<Auction, AuctionDto>().IncludeMembers(auction => auction.Item);
    CreateMap<Item, AuctionDto>();
    CreateMap<CreateAuctionDto, Auction>()
      .ForMember(auction => auction.Item, options => options.MapFrom(item => item));
    CreateMap<CreateAuctionDto, Item>();
    CreateMap<AuctionDto, AuctionCreated>();
    CreateMap<Auction, AuctionUpdated>().IncludeMembers(auction => auction.Item);
    CreateMap<Item, AuctionUpdated>();


  }
}
