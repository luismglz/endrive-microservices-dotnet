using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("endriveApp","Endrive app full access")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client {
                ClientId = "postman",
                ClientName = "Postman",
                AllowedScopes = {"openid", "profile", "endriveApp" },
                RedirectUris = {"https://www.getpostman.com/oauth/callback"},
                ClientSecrets = new []{new Secret("NotASecret".Sha256())},
                AllowedGrantTypes = {GrantType.ResourceOwnerPassword}
            }
        };
}
