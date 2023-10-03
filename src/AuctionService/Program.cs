using AuctionService.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Connections;
using System.Diagnostics;
using MassTransit;
using Consumers;

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
  config.AddEntityFrameworkOutbox<AuctionDbContext>(options =>
  {
    options.QueryDelay = TimeSpan.FromSeconds(10);
    options.UsePostgres();
    options.UseBusOutbox();
  });

  config.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

  config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

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
