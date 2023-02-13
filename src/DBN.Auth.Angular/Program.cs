using DBN.Auth.Auth0;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthApi(new AuthApiParameters(
    domain: $"{builder.Configuration["Auth0:Domain"]}",
    audience: $"{builder.Configuration["Auth0:Audience"]}",
    authorizationPolicy: new AuthPolicyParameter("read:data", "read:data")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAuthApi(
    auth0Domain: $"{builder.Configuration["Auth0:Domain"]}",
    auth0Audience: $"{builder.Configuration["Auth0:Audience"]}",
    client: new NetworkCredential($"{builder.Configuration["Auth0:ClientId"]}", builder.Configuration["Auth0:ClientSecret"], $"{builder.Configuration["Auth0:Domain"]}"),
    route: "/api/auth"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
