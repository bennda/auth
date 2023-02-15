using System.Net;
using DBN.Auth.Auth0;

var builder = WebApplication.CreateBuilder(args);

var config = new { 
    Domain = $"{builder.Configuration["Auth0:Domain"]}",
    Audience = $"{builder.Configuration["Auth0:Audience"]}",
    AuthorizationPolicies = new[]
    {
        new AuthPolicyParameter(
            name: "read",
            claim: new AuthPolicyClaim("permissions", "read:data"))
    },
    Client = new NetworkCredential($"{builder.Configuration["Auth0:ClientId"]}", builder.Configuration["Auth0:ClientSecret"])
};

builder.Services.AddAuthApi(new AuthApiParameters(
    domain: config.Domain,
    audience: config.Audience,
    authorizationPolicies: config.AuthorizationPolicies));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Map("/", () =>
{
    return Results.Empty;
});

// configure auth api
app.UseAuthApi(
    auth0Domain: config.Domain,
    auth0Audience: config.Audience,
    client: config.Client,
    route: "/token"
);

app.MapGet("/public", () =>
{
    return "hello 1";
});

app.MapGet("/protected", () => {
    return "hello 2";
}).RequireAuthorization("read");

app.Run();