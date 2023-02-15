namespace DBN.Auth.Auth0;

public static class Auth0Extensions
{
    public static IServiceCollection AddAuthApi(this IServiceCollection services, AuthApiParameters parameters)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
            {
                c.Authority = $"https://{parameters.Domain}";
                c.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = parameters.Audience,
                    ValidIssuer = parameters.Domain
                };
            });
        services.AddAuthorization(o =>
        {
            if (parameters.AuthorizationPolicies != null)
            {
                foreach (var policy in parameters.AuthorizationPolicies)
                {
                    foreach (var claim in policy.Claims)
                    {
                        o.AddPolicy(policy.Name, p => p
                            .RequireAuthenticatedUser()
                            .Requirements.Add(new HasClaimRequirement(claim, parameters.Domain)));
                    }
                }
            }
        });

        services.AddSingleton<IAuthorizationHandler, HasClaimHandler>();
        services.AddHttpClient<ITokenService, Auth0TokenService>();
        return services;
    }

    public static IServiceCollection AddAuthApp(this IServiceCollection services, AuthAppParameters parameters)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.OnAppendCookie = (context) =>
            {
                if (context.CookieOptions.SameSite == SameSiteMode.None && context.CookieOptions.Secure)
                {
                    context.CookieOptions.SameSite = SameSiteMode.Unspecified;
                }
            };
            options.OnDeleteCookie = (context) =>
            {
                if (context.CookieOptions.SameSite == SameSiteMode.None && context.CookieOptions.Secure)
                {
                    context.CookieOptions.SameSite = SameSiteMode.Unspecified;
                }
            };
            options.CheckConsentNeeded = contenxt => true;
        });

        services.AddAuth0WebAppAuthentication(options =>
        {
            options.Domain = $"{parameters.Domain}";
            options.ClientId = $"{parameters.ClientId}";
            options.CallbackPath = $"{parameters.CallbackPath}";
            options.Scope = $"{parameters.Scope}";

            if (!string.IsNullOrEmpty(parameters.CallbackHost))
            {
                options.OpenIdConnectEvents = new OpenIdConnectEvents
                {
                    OnTokenValidated = (context) =>
                    {
                        if (parameters.PermissionRoles != null)
                        {
                            foreach (var scope in context.Options.Scope.Where(p => parameters.PermissionRoles.Contains(p)))
                            {
                                (context.Principal?.Identity as ClaimsIdentity)?.AddClaim(new Claim(ClaimTypes.Role.ToString(), scope));
                            }
                        }

                        return Task.CompletedTask;
                    },

                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.RedirectUri = $"{parameters.CallbackHost}{parameters.CallbackPath}";
                        return Task.FromResult(0);
                    }
                };
            }
        });

        services.ConfigureSameSiteNoneCookies();

        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, AuthApiParameters apiParameters, AuthAppParameters appParameters)
    {
        services.AddAuthApi(apiParameters);
        services.AddAuthApp(appParameters);
        return services;
    }


    public static IApplicationBuilder UseAuth(this WebApplication app, string auth0Domain, string auth0Audience, NetworkCredential client, string route = "/auth")
    {
        app.UseAuthApi(auth0Domain, auth0Audience, client, route);
        app.UseAuthApp();
        return app;
    }

    public static IApplicationBuilder UseAuthApi(this WebApplication app, string auth0Domain, string auth0Audience, NetworkCredential client, string route = "/auth")
    {
        var handler = async (HttpRequest request, ITokenService authService) =>
        {
            Dictionary<string, string?>? content = null;

            try
            {
                content = await JsonSerializer.DeserializeAsync<Dictionary<string, string?>>(request.Body);
            }
            catch (Exception)
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
                response = await authService.GetToken(TokenParameters.ForClientCredentials(
                    domain: $"{domain}",
                    audience: $"{audience}",
                    client: new NetworkCredential(clientId, clientSecret, domain)));
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
                    response = await authService.GetToken(TokenParameters.ForPassword(
                        domain: $"{domain}",
                        audience: $"{audience}",
                        client: new NetworkCredential(clientId, clientSecret, domain),
                        user: new NetworkCredential(username, password, domain),
                        scope: $"{scope}"
                        ));

                    break;
                case "refresh_token":
                    var refreshToken = content.GetValueOrDefault("refresh_token");
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        return Results.BadRequest();
                    }
                    response = await authService.GetToken(TokenParameters.ForRefreshToken(
                        domain: $"{domain}",
                        client: new NetworkCredential(clientId, clientSecret, domain),
                        refreshToken: refreshToken));
                    break;
                case "authorization_code":
                    var code = content.GetValueOrDefault("code");
                    var redirectUri = content.GetValueOrDefault("redirect_uri");
                    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(redirectUri))
                    {
                        return Results.BadRequest();
                    }
                    response = await authService.GetToken(TokenParameters.ForAuthorizationCode(
                        domain: $"{domain}",
                        client: new NetworkCredential(clientId, clientSecret, domain),
                        code: code,
                        redirectUri: redirectUri));
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

    public static IApplicationBuilder UseAuthApp(this WebApplication app)
    {
        app.UseCookiePolicy();
        return app;
    }
}