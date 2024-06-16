using System.Collections.Concurrent;

using AspNetCoreSample.DataModel.Models;
using AspNetCoreSample.WebApi;
using AspNetCoreSample.WebApi.Hubs;
using AspNetCoreSample.WebApi.Options;
using AspNetCoreSample.WebApi.Resources;
using AspNetCoreSample.WebApi.Services.Keycloak.Admin;
using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "";
builder.Services.AddDbContext<SampleContext>(
    options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddHttpClient();

var keycloakSection = builder.Configuration.GetSection(nameof(KeycloakOptions));
builder.Services.Configure<KeycloakOptions>(keycloakSection);
var keycloakOptions = keycloakSection.Get<KeycloakOptions>()!;

builder.Services.AddSignalR();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.Authority = keycloakOptions.Authority;
        options.Audience = keycloakOptions.Audience;
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization();

builder.Services.Configure<PolicyOptions>(builder.Configuration.GetSection(nameof(PolicyOptions)));
var policyCache = new ConcurrentDictionary<string, AuthorizationPolicy>();
builder.Services.AddSingleton(policyCache);
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
builder.Services.AddSingleton<PolicyService>();

builder.Services.AddValidatorsFromAssemblyContaining<TokenRequestValidator>();

var corsSection = builder.Configuration.GetSection(CorsOptions.Position).Get<CorsOptions>()!;

builder.Services.AddCors(options => // Add this line
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod();

        builder.WithOrigins("https://localhost:7079")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod();

        builder.WithOrigins("http://localhost:3000")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod();

        builder.WithOrigins(corsSection.MvcUrl).AllowAnyHeader().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "AspNetCoreSample WebApi";
    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Bearer {token}"
    });

    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
    // OpenAPIでAccept-Languageを切り替える設定を追加
    configure.OperationProcessors.Add(new AcceptLanguageHeaderParameter());
});

// Localizationを有効化
// Query String、Cookie、Accept-Languageヘッダーで決める
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IKeycloakService, KeycloakService>();
builder.Services.AddSingleton<ISharedResource, SharedResource>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseOpenApi(); // serve OpenAPI/Swagger documents
app.UseSwaggerUi(); // serve Swagger UI
app.UseReDoc(); // serve ReDoc UI
// }

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseCors(); // Add this line

app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(options =>
{
    //サポートするカルチャの設定
    string[] supportedCultures = ["ja", "en"];

    options
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures)
        .SetDefaultCulture(supportedCultures[0]);
});

app.MapControllers();

app.MapHub<QrCodeHub>("/qrcodeHub");

app.Run();

/// <summary>
/// Testプロジェクトから参照するためTop Level Statementをpublicにする
/// </summary>
public partial class Program { }
