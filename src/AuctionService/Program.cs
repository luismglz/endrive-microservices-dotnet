using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuctionService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the app instance container.
builder.Services.AddControllers();
var auctionsSettings = builder.Configuration["Auctions:ConnectionSettings"];
var IdentityServiceUrl = builder.Configuration["Auctions:IdentityServiceUrl"];

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

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGrpcService<GrpcAuctionService>();

try
{
  DbInitializer.InitDb(app);
}
catch (Exception e)
{
  Console.WriteLine(e);
}

app.Run();
