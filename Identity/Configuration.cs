using System;
using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityModel;
using IdentityServer4;

namespace Identity
{
    public static class Configuration
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
			   new ApiScope("WebAPI", "Web API"),
		       new ApiScope("SignalRHub", "SignalR Hub")
			};
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("WebAPI", "Web API", new [] {JwtClaimTypes.Name})
            };
        public static IEnumerable<Client> Clients =>
                new List<Client>
                {
                    new Client
                    {
                        ClientId = "...",
                        ClientName = "API Client",
					    AllowedGrantTypes = GrantTypes.ClientCredentials,
						ClientSecrets =
			            {
				            new Secret("secret".Sha256())
			            },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "WebAPI"
                        }
                    },
					new Client
		            {
			            ClientId = "signalr-client",
			            ClientName = "SignalR Client",
			            AllowedGrantTypes = GrantTypes.ClientCredentials,
			            ClientSecrets =
			            {
				            new Secret("secret".Sha256())
			            },
						 AllowedScopes =
						{
							IdentityServerConstants.StandardScopes.OpenId,
							IdentityServerConstants.StandardScopes.Profile,
							"SignalRHub"
						}
		            }
				};
    }
}