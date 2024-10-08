using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace HybridFlow;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new IdentityResources.Email(),
            new IdentityResource(
                "roles",
                "Your role(s)",
                ["role"])
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new("microserviceAPI", "Micro ServiceAPI API"),
        ];

    public static IEnumerable<Client> Clients =>
        [
            new()
            {
                ClientId = "Web_client",
                ClientName = "Web App",
                AllowedGrantTypes = GrantTypes.Hybrid,
                RequirePkce = false,
                AllowRememberConsent = false,
                //AllowAccessTokensViaBrowser = false,
                RedirectUris = new List<string>
                {
                    "https://localhost:6065/signin-oidc"  // -- > MVC Client App port 
                },
                PostLogoutRedirectUris = new List<string>
                {
                    " https://localhost:6065/" // --> MVC Client App port
                },
                ClientSecrets = new List<Secret>
                {
                    new("secret".Sha256())
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Address,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "microserviceAPI"
                },
            }
        ];
}
