using System.Net;
using System.Text;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(config =>
{

  config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

  config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

  config.UsingRabbitMq((context, cfg) =>
  {
    string queueEndpointName = builder.Configuration["Search:Queue:AuctionCreated"];

    cfg.ReceiveEndpoint(queueEndpointName, endpoint =>
    {
      endpoint.UseMessageRetry(retry => retry.Interval(5, 5));
      endpoint.ConfigureConsumer<AuctionCreatedConsumer>(context);
    });


    cfg.ConfigureEndpoints(context);
  });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
  try
  {
    await DbInitializer.InitDb(app, builder);
  }
  catch (Exception e)
  {
    Console.WriteLine(e);
  }
});



app.Run();

//checks the availability of other microservices and retry util works again
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
  => HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(result => result.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromMinutes(3));

