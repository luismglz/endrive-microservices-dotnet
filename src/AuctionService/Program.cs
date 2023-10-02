using AuctionService.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Connections;
using System.Diagnostics;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
var auctionsSettings = builder.Configuration["Auctions:ConnectionSettings"];

builder.Services.AddDbContext<AuctionDbContext>(options =>
{
  options.UseNpgsql(auctionsSettings);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(config =>
{
  config.UsingRabbitMq((context, cfg) =>
  {
    cfg.ConfigureEndpoints(context);
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

try
{
  DbInitializer.InitDb(app);
}
catch (Exception e)
{
  Console.WriteLine(e);
}

app.Run();
