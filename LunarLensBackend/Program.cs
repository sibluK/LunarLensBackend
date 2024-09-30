using System.Security.Claims;
using System.Text;
using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Swagger;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

Env.Load();
var bld = WebApplication.CreateBuilder();

bld.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("JwtBearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))),
            ValidIssuer = bld.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = bld.Configuration.GetSection("Jwt:Audience").Value,
            RoleClaimType = ClaimTypes.Role
        };
    })
    .AddOpenIdConnect("Microsoft", options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{bld.Configuration.GetSection("AzureAd:TenantId").Value}/v2.0";
        options.ClientId = bld.Configuration.GetSection("AzureAd:ClientId").Value;
        options.ClientSecret = bld.Configuration.GetSection("AzureAd:ClientSecret").Value;
        options.ResponseType = "code"; 
        options.SaveTokens = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = bld.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = bld.Configuration.GetSection("Jwt:Audience").Value,
            RoleClaimType = ClaimTypes.Role,
        };
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                context.Response.StatusCode = 401;
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    });
    //.AddMicrosoftIdentityWebApi(bld.Configuration.GetSection("AzureAd"), "AzureAdBearer");

bld.Services.AddFastEndpoints().SwaggerDocument();

bld.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500", "http://localhost:5131")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

bld.Services.AddDbContextFactory<Context>(options =>
{
    options.UseNpgsql("User ID=postgres; Password=1324; Database=lunarlens; Server=localhost; Port=5432; Include Error Detail=true;");
});

bld.Logging.AddConsole();

bld.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Context>();

bld.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
        policy.AddAuthenticationSchemes("JwtBearer", "Microsoft");
    })
    .AddPolicy("EditorOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Editor");
        policy.AddAuthenticationSchemes("JwtBearer", "Microsoft");
    })
    .AddPolicy("BasicUserOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("BasicUser");
        policy.AddAuthenticationSchemes("JwtBearer", "Microsoft");
    });

var app = bld.Build();

await RoleSeeder.SeedRolesAndAdminUserAsync(app.Services);

app.UseCors("AllowLocalhost");

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Path}");
    await next.Invoke();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();
