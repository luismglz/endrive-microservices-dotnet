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
            },
            new Client {
                ClientId ="nextApp",
                ClientName ="nextApp",
                ClientSecrets = {new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RequirePkce = false,
                RedirectUris = {"http://localhost:3000/api/auth/callback/id-server"},
                AllowOfflineAccess = true,
                AllowedScopes = {"openid", "profile", "endriveApp"},
                AccessTokenLifetime = 3600*24*30
            }
        };
}
