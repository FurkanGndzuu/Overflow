using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection service)
        {
            service.AddAuthentication().AddKeycloakJwtBearer("keycloak", "overflow", options =>
            {
                options.Audience = "overflow";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuers = [
                                "http://localhost:6001/realms/overflow",
                            "http://keycloak:8080/realms/overflow",
                            "http://id.overflow.local/realms/overflow",
                        ]
                };
            });
            return service;
        }
    }
}
