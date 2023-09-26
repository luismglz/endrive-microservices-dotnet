using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{

  private readonly AuctionDbContext _context;
  private readonly IMapper _mapper;

  public AuctionsController(AuctionDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
  {
    var auctions = await _context.Auctions
      .Include(auction => auction.Item)
      .OrderBy(auction => auction.Item.Make)
    .ToListAsync();

    return _mapper.Map<List<AuctionDto>>(auctions);
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

    if (!isSuccessResult) return BadRequest("Could not save changes");

    return CreatedAtAction(
      nameof(GetAuctionById),
      new { auction.Id }, _mapper.Map<AuctionDto>(auction)
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

}
