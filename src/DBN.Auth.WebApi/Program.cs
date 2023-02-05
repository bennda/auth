using DBN.Auth.Auth0;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthApi(
    domain: $"{builder.Configuration["Auth0:Domain"]}", 
    audience: $"{builder.Configuration["Auth0:Audience"]}"
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Map("/", () => {
    return Results.Empty;
});

// configure auth api
app.UseAuthApi(
    auth0Domain: $"{builder.Configuration["Auth0:Domain"]}", 
    auth0Audience: $"{builder.Configuration["Auth0:Audience"]}",
    client: new NetworkCredential($"{builder.Configuration["Auth0:ClientId"]}", builder.Configuration["Auth0:ClientSecret"]),
    route: "/token"
);

app.Run();