using BiddingService;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;

var builder = WebApplication.CreateBuilder(args);
var biddingServiceSettings = builder.Configuration["Bidding:ConnectionSettings"];

// Add services to the container.

builder.Services.AddControllers();

var auctionsSettings = builder.Configuration["Auctions:ConnectionSettings"];
var IdentityServiceUrl = builder.Configuration["Bidding:IdentityServiceUrl"];

builder.Services.AddMassTransit(config =>
{

  config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
  config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));
  config.UsingRabbitMq((context, cfg) =>
  {

    cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
    {
      host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
      host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
    });

    cfg.ConfigureEndpoints(context);
  });

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  options.Authority = IdentityServiceUrl;
  options.RequireHttpsMetadata = false;
  options.TokenValidationParameters.ValidateAudience = false;
  options.TokenValidationParameters.NameClaimType = "username";

});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHostedService<CheckAuctionFinished>();
builder.Services.AddScoped<GrpcAuctionClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();



await DB.InitAsync("bidDB", MongoClientSettings.FromConnectionString(biddingServiceSettings));

app.Run();
