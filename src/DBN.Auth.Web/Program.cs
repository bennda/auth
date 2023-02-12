var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuth(
    new AuthApiParameters(
        domain: $"{builder.Configuration["Auth0:Domain"]}",
        audience: $"{builder.Configuration["Auth0:Audience"]}",
        authorizationPolicy: new AuthPolicyParameter("read:data", "read:data")
    ),
    new AuthAppParameters(
        domain: $"{builder.Configuration["Auth0:Domain"]}",
        audience: $"{builder.Configuration["Auth0:Audience"]}",
        clientId: $"{builder.Configuration["Auth0:ClientId"]}",
        scope: $"{builder.Configuration["Auth0:Scope"]}",
        callbackPath: $"{builder.Configuration["Auth0:CallbackPath"]}",
        callbackHost: $"{builder.Configuration["Auth0:CallbackHost"]}",
        permissionRoles: $"{builder.Configuration["Auth0:Roles"]}".Split(' ', StringSplitOptions.RemoveEmptyEntries)
    )
);

//builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Views/Token/Grants/{0}.cshtml");
    });
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Token/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAuth(
    auth0Domain: $"{builder.Configuration["Auth0:Domain"]}",
    auth0Audience: $"{builder.Configuration["Auth0:Audience"]}",
    client: new NetworkCredential($"{builder.Configuration["Auth0:ClientId"]}", builder.Configuration["Auth0:ClientSecret"])
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Token}/{action=Index}/{id?}");

app.Run();
