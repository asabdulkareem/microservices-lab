using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Shopping.Web.HttpHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient("IDFClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:6066/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
        c.DefaultRequestHeaders.Clear();
        c.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
        c.DefaultRequestHeaders.Clear();
        c.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
        c.DefaultRequestHeaders.Clear();
        c.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = builder.Configuration["ApiSettings:IdentityServerAddress"];
                options.ClientId = "Web_client";
                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";

                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("microserviceAPI");
                options.Scope.Add("address");
                options.Scope.Add("email");
                options.Scope.Add("roles");

                options.ClaimActions.MapUniqueJsonKey("role", "role");

                options.SaveTokens = true;

                options.GetClaimsFromUserInfoEndpoint = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
