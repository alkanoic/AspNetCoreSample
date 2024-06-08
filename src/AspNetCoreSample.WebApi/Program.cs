using AspNetCoreSample.WebApi.EfModels;
using AspNetCoreSample.WebApi.Hubs;
using AspNetCoreSample.WebApi.Options;
using AspNetCoreSample.WebApi.Services.Keycloak.Admin;
using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Value == "user" || c.Value == "admin"
            )
        );
    });

    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Value == "admin"
            )
        );
    });
});

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
});

builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IKeycloakService, KeycloakService>();

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

app.MapControllers();

app.MapHub<QrCodeHub>("/qrcodeHub");

app.Run();

/// <summary>
/// Testプロジェクトから参照するためTop Level Statementをpublicにする
/// </summary>
public partial class Program { }
