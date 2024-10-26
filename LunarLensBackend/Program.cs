using System.Security.Authentication.ExtendedProtection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Swagger;
using LunarLensBackend.Database;
using LunarLensBackend.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

Env.Load();
var bld = WebApplication.CreateBuilder();

bld.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("JwtBearer", options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{bld.Configuration["AzureAd:TenantId"]}/v2.0";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))),
            ValidIssuers = new[]
            {
                $"https://login.microsoftonline.com/{bld.Configuration.GetSection("AzureAd:TenantId").Value}/v2.0",
                "http://localhost:5131"
            },
            ValidAudiences = new[]
            {
                bld.Configuration.GetSection("Jwt:Audience").Value,
                bld.Configuration.GetSection("AzureAd:ClientId").Value
            },
            RoleClaimType = ClaimTypes.Role
        };
    })
    .AddMicrosoftIdentityWebApi(bld.Configuration.GetSection("AzureAd"));

bld.Services.Configure<JwtBearerOptions>("JwtBearer", options =>
{
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var issuer = context.Principal.FindFirst(JwtRegisteredClaimNames.Iss)?.Value;

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<Context>();
            
            if (issuer != null && issuer.Contains("login.microsoftonline.com"))
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.MicrosoftId == userId);

                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        MicrosoftId = userId,
                        UserName = context.Principal.FindFirst("preferred_username")?.Value,
                        Email = context.Principal.FindFirst("preferred_username")?.Value
                    };

                    await dbContext.Users.AddAsync(user);
                    await dbContext.SaveChangesAsync();
                    
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    await userManager.AddToRoleAsync(user, "BasicUser");

                    await dbContext.SaveChangesAsync();
                }
            }
            await Task.CompletedTask;
        }
    };
});

bld.Services.AddFastEndpoints().SwaggerDocument().AddResponseCaching();

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

bld.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Context>();

bld.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
        policy.AddAuthenticationSchemes("JwtBearer");
    })
    .AddPolicy("EditorOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Editor");
        policy.AddAuthenticationSchemes("JwtBearer");
    })
    .AddPolicy("BasicUserOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("BasicUser");
        policy.AddAuthenticationSchemes("JwtBearer");
    })
    .AddPolicy("WriterOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Writer");
        policy.AddAuthenticationSchemes("JwtBearer");
    });


bld.Services.AddScoped<TokenGeneration>();

bld.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
/*app.MapIdentityApi<ApplicationUser>();*/
app.UseResponseCaching().UseFastEndpoints();
app.UseSwaggerGen();

app.Run();
