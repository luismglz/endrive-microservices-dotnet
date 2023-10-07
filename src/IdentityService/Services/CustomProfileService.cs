using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService;
public class CustomProfileService : IProfileService
{

  private readonly UserManager<ApplicationUser> _userManager;


  public CustomProfileService(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }


  //Request or add additional info to token
  public async Task GetProfileDataAsync(ProfileDataRequestContext context)
  {
    var user = await _userManager.GetUserAsync(context.Subject);
    var existingClaims = await _userManager.GetClaimsAsync(user);

    var claims = new List<Claim>{
      new Claim("username", user.UserName)
    };

    context.IssuedClaims.AddRange(claims);
    context.IssuedClaims.Add(existingClaims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.GivenName));
    context.IssuedClaims.Add(existingClaims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.FamilyName));
  }

  public Task IsActiveAsync(IsActiveContext context)
  {
    return Task.CompletedTask;
  }
}
