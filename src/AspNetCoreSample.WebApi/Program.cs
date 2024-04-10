using System.Security.Claims;
using System.Text;

using AspNetCoreSample.WebApi.Hubs;
using AspNetCoreSample.WebApi.Options;
using AspNetCoreSample.WebApi.Services.Token;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient();

var keycloakSection = builder.Configuration.GetSection(KeycloakOptions.Position);
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
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
