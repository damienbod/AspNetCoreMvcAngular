// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;

namespace QuickstartIdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("thingsscope",new []{ "role", "admin", "user", "thingsapi" } )
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("thingsscope")
                {
                    ApiSecrets =
                    {
                        new Secret("thingsscopeSecret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "thingsscope",
                            DisplayName = "Scope for the thingsscope ApiResource"
                        }
                    },
                    UserClaims = { "role", "admin", "user", "thingsapi" }
                }
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientName = "angularmvcmixedclient",
                    ClientId = "angularmvcmixedclient",
                    AccessTokenType = AccessTokenType.Reference,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44341"

                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:44341/"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:44341/"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "thingsscope",
                        "role"
                    }
                }
            };
        }
    }
}