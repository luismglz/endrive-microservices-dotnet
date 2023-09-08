using AuctionService.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Connections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
var auctionsSettings = builder.Configuration["Auctions:ConnectionSettings"];

builder.Services.AddDbContext<AuctionDbContext>(options => {
  options.UseNpgsql(auctionsSettings);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();
