using AspNetCoreSample.DataModel.Models;
using AspNetCoreSample.Mvc.Models;
using AspNetCoreSample.Mvc.Options;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using WebPush;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "";
builder.Services.AddDbContext<SampleContext>(
    options => options.UseNpgsql(connectionString));

builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<FluentViewModel>();

builder.Services.AddHttpClient();

var keycloakOptions = builder.Configuration.GetSection(KeycloakOptions.Position).Get<KeycloakOptions>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie("Cookies")
  .AddOpenIdConnect(options =>
  {
      options.Authority = keycloakOptions.Authority;
      options.MetadataAddress = keycloakOptions.MetadataAddress;
      options.ClientId = keycloakOptions.ClientId;
      options.ClientSecret = keycloakOptions.ClientSecret;
      options.Scope.Add("openid");
      options.Scope.Add("profile");
      options.ResponseType = OpenIdConnectResponseType.Code;
      //   options.SaveTokens = true;
      //   options.GetClaimsFromUserInfoEndpoint = true;
      //   options.TokenValidationParameters = new TokenValidationParameters
      //   {
      //       NameClaimType = "name",
      //       RoleClaimType = "role"
      //   };
      // 開発のためHttpを許可する
      options.RequireHttpsMetadata = false;
  });
builder.Services.AddAuthorization();

var vapidKeys = VapidHelper.GenerateVapidKeys();
var vapidOption = new AspNetCoreSample.Mvc.Options.VapidOption()
{
    PublicKey = vapidKeys.PublicKey,
    PrivateKey = vapidKeys.PrivateKey
};

builder.Services.AddSingleton(vapidOption);

builder.Services.Configure<WebApiOption>(builder.Configuration.GetSection(WebApiOption.Position));
builder.Services.Configure<JavaScriptOptions>(builder.Configuration.GetSection(nameof(JavaScriptOptions)));

builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyJsFiles("js/**/*.js");
    pipeline.MinifyCssFiles("css/**/*.css");
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "SampleInstance";
});

builder.Services.AddSession(options =>
{
    // セッションの有効期限を設定
    options.IdleTimeout = TimeSpan.FromSeconds(20);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseWebOptimizer();
}

app.MapDefaultEndpoints();

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
