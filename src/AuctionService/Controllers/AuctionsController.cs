using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{

  private readonly AuctionDbContext _context;
  private readonly IMapper _mapper;
  private readonly IPublishEndpoint _publishEndpoint;

  public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
  {
    _context = context;
    _mapper = mapper;
    _publishEndpoint = publishEndpoint;
  }

  [HttpGet]
  public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
  {

    var query = _context.Auctions.OrderBy(auction => auction.Item.Make).AsQueryable();

    if (!string.IsNullOrEmpty(date))
    {
      query = query.Where(auction => auction.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
    }

    return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
  }



  [HttpGet("{id}")]
  public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
  {
    var auction = await _context.Auctions
      .Include(auction => auction.Item)
      .FirstOrDefaultAsync(auction => auction.Id == id);

    if (auction is null) return NotFound();


    return _mapper.Map<AuctionDto>(auction);

  }


  [HttpPost]
  public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
  {

    var auction = _mapper.Map<Auction>(auctionDto);
    auction.Seller = "Test";
    _context.Auctions.Add(auction);
    var isSuccessResult = await _context.SaveChangesAsync() > 0;

    var newAuction = _mapper.Map<AuctionDto>(auction);

    //map created auction to be published
    await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

    if (!isSuccessResult) return BadRequest("Could not save changes");

    return CreatedAtAction(
      nameof(GetAuctionById),
      new { auction.Id }, newAuction
      );

  }


  [HttpPut("{id}")]
  public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
  {

    var auction = await _context.Auctions.Include(auction => auction.Item)
    .FirstOrDefaultAsync(auction => auction.Id == id);


    if (auction is null) return NotFound();

    //TODO: check seller == username

    auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
    auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
    auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
    auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
    auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

    var result = await _context.SaveChangesAsync() > 0;

    if (result) return Ok();

    return BadRequest("There was an error at update");
  }


  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteAuction(Guid id)
  {
    var auction = await _context.Auctions.FindAsync(id);

    if (auction is null) return NotFound();

    //TODO: check seller == username

    _context.Auctions.Remove(auction);
    var result = await _context.SaveChangesAsync() > 0;

    if (!result) return BadRequest("There was an error in deleting the auction");

    return Ok();
  }

}
