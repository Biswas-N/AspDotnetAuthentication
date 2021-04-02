using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IdentityServer
{
    public static class IdentityServerConfig
    {
        public static List<TestUser> TestUsers
        {
            get
            {
                var address = new
                {
                    street_address = "One Hacker Way",
                    locality = "Heidelberg",
                    postal_code = 69118,
                    country = "Germany"
                };

                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "100001",
                        Username = "alice",
                        Password = "alice",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.Role, "admin"),
                            new Claim(JwtClaimTypes.WebSite, "https://alice.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                                IdentityServerConstants.ClaimValueTypes.Json)
                        }
                    }
                };
            }
        }

        public static IEnumerable<Client> Clients => 
            new []
            {
                new Client
                {
                    ClientId = "m2m.webapp",
                    ClientName = "MVC Web App",
                        
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},
                        
                    AllowedScopes = {"weatherapi.read", "weatherapi.write"}
                }
            };
        
        public static IEnumerable<ApiScope> ApiScopes =>
            new []
            {
                new ApiScope("weatherapi.read"),
                new ApiScope("weatherapi.write"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new []
            {
                new ApiResource("weatherapi")
                {
                    Scopes = {"weatherapi.read", "weatherapi.write"},
                    ApiSecrets = {new Secret("ScopeSecret".Sha256())},
                    UserClaims = {"role"}
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = {"role"}
                }
            };
    }
}