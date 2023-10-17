using MassTransit;
using NotificationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(config =>
{
  config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));
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

builder.Services.AddSignalR();


var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();