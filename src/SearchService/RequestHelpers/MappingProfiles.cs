using AutoMapper;
using Contracts;
using SearchService.Models;

namespace RequestHelpers;
public class MappingProfiles : Profile
{
  public MappingProfiles()
  {
    CreateMap<AuctionCreated, Item>();
  }

}
