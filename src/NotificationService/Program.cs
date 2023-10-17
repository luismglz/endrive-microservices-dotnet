using MassTransit;

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


var app = builder.Build();



app.Run();
