namespace DBN.Auth.Auth0;

public static class Auth0Extensions
{
    public static IServiceCollection AddAuthApi(this IServiceCollection services, string domain, string audience)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
            {
                c.Authority = $"https://{domain}";
                c.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = audience,
                    ValidIssuer = domain
                };
            });
        services.AddAuthorization(o =>
        {
            o.AddPolicy("read:data", p => p
                .RequireAuthenticatedUser()
                .Requirements.Add(new HasScopeRequirement("read:data", $"https://{domain}/"))
                );
        });

        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        services.AddHttpClient<IAuthService, Auth0Service>();
        return services;
    }

    public static IServiceCollection AddAuthApp(this IServiceCollection services, string domain, string clientId, string callbackPath, string callbackHost)
    {   
        services.AddAuth0WebAppAuthentication(options =>
        {
            options.Domain = domain;
            options.ClientId = clientId;
            options.CallbackPath = callbackPath;

            if (!string.IsNullOrEmpty(callbackHost))
            {
                options.OpenIdConnectEvents = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context => {
                        context.ProtocolMessage.RedirectUri = $"{callbackHost}{callbackPath}";
                        return Task.FromResult(0);
                    }
                };
            }
        });
        
        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, string domain, string audience, string scope, string clientId, string callbackPath, string callbackHost)
    {
        services.AddAuthApi(domain, audience);
        services.AddAuthApp(domain, clientId, callbackPath, callbackHost);
        
        return services;
    }

    public static IApplicationBuilder UseAuthApi(this WebApplication app, string auth0Domain, string auth0Audience, NetworkCredential client, string route = "/token")
    {
        Delegate handler = async (HttpRequest request, IAuthService authService) =>
        {
            Dictionary<string, string?>? content = null;

            try
            {
                content = await JsonSerializer.DeserializeAsync<Dictionary<string, string?>>(request.Body);
            } catch (Exception)
            {
                return Results.BadRequest();
            }
            
            if (content == null || !content.TryGetValue("grant_type", out string? grantType))
            {
                return Results.BadRequest();
            }

            var getValue = new Func<Dictionary<string, string?>, string, string, string?>((req, key, defaultValue) => req.TryGetValue(key, out string? val)
                ? val : defaultValue
            );
            var domain = content.GetValueOrDefault("domain", auth0Domain);
            var audience = content.GetValueOrDefault("audience", auth0Audience);
            var clientId = content.GetValueOrDefault("client_id", null);
            var clientSecret = content.GetValueOrDefault("client_secret", null);

            TokenResponse? response = null;

            // handle client credentials request
            if (grantType == "client_credentials")
            {
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    return Results.BadRequest();
                }
                response = await authService.GetTokenForClientCredentials($"{domain}", $"{audience}", new NetworkCredential(clientId, clientSecret, domain));
            }

            // check request types
            switch (grantType)
            {
                case "password":
                case "refresh_token":
                case "authorization_code":
                    if (string.IsNullOrEmpty(clientId))
                    {
                        clientId = $"{client.UserName}";
                    }
                    if (string.IsNullOrEmpty(clientSecret))
                    {
                        clientSecret = $"{client.Password}";
                    }
                    break;
                case "client_credentials":
                    break;
                default:
                    return Results.BadRequest("grant_type invalid");
            }

            // handle request types
            switch (grantType)
            {
                case "password":
                    var username = content.GetValueOrDefault("username");
                    var password = content.GetValueOrDefault("password");
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(audience))
                    {
                        return Results.BadRequest();
                    }
                    var scope = content.GetValueOrDefault("scope", "openId");
                    response = await authService.GetTokenForPassword($"{domain}", $"{audience}", new NetworkCredential(clientId, clientSecret, domain), new NetworkCredential(username, password, domain), $"{scope}");
                    break;
                case "refresh_token":
                    var refreshToken = content.GetValueOrDefault("refresh_token");
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        return Results.BadRequest();
                    }
                    response = await authService.GetTokenForRefreshToken($"{domain}", new NetworkCredential(clientId, clientSecret, domain), refreshToken);
                    break;
                case "authorization_code":
                    var code = content.GetValueOrDefault("code");
                    var redirectUri = content.GetValueOrDefault("redirect_uri");
                    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(redirectUri))
                    {
                        return Results.BadRequest();
                    }
                    response = await authService.GetTokenForAuthorizationCode($"{domain}", new NetworkCredential(clientId, clientSecret, domain), code, redirectUri);
                    break;
                case "client_credentials":
                    break;
                default:
                    return Results.BadRequest("grant_type invalid");
            }

            return response != null && response.IsSuccessful
                ? Results.Ok(response.Token)
                : Results.Unauthorized();
        };
        app.MapPost(route, handler);

        return app;
    }
}
