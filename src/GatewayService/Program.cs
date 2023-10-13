using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var IdentityServiceUrl = builder.Configuration["Auctions:IdentityServiceUrl"];


builder.Services.AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  options.Authority = IdentityServiceUrl;
  options.RequireHttpsMetadata = false;
  options.TokenValidationParameters.ValidateAudience = false;
  options.TokenValidationParameters.NameClaimType = "username";

});


var app = builder.Build();

app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
