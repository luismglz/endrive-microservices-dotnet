﻿using AuctionService.Data;
using AuctionService.DTOs;
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

}
