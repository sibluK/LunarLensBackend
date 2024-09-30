using System.Security.Claims;
using System.Text;
using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Swagger;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

Env.Load();
var bld = WebApplication.CreateBuilder();

bld.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"))),
        ValidIssuer = bld.Configuration.GetSection("Jwt:Issuer").Value,
        ValidAudience = bld.Configuration.GetSection("Jwt:Audience").Value,
        RoleClaimType = ClaimTypes.Role
    };

    /*// Override challenge behavior to prevent redirection to /Account/Login
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse(); // Prevents redirect
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    };*/
});

/*bld.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();*/

// Disable cookie authentication redirects
bld.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = PathString.Empty;
    options.AccessDeniedPath = PathString.Empty;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

bld.Services.AddFastEndpoints().SwaggerDocument();

bld.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
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
        //policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    })
    .AddPolicy("EditorOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Editor");
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
