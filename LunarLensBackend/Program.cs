using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

Env.Load();
var bld = WebApplication.CreateBuilder();
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
Console.WriteLine("JWT_SECRET: " + jwtSecret);

bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument();
bld.Services.AddAuthenticationJwtBearer(x => x.SigningKey = jwtSecret);
bld.Services.AddAuthorization();
bld.Logging.AddConsole(); // Add this to enable console logging


bld.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

bld.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 3;
    })
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

bld.Services.AddDbContextFactory<Context>(options =>
{
    options.UseNpgsql("User ID=postgres; Password=1324; Database=lunarlens; Server=localhost; Port=5432; Include Error Detail=true;");
});

bld.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EditorOnly", policy => policy.RequireRole("Editor"));
});

var app = bld.Build();

await SeedRolesAndAdminUserAsync(app.Services);

app.UseCors("AllowLocalhost"); 
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseSwaggerGen();


app.Use(async (context, next) =>
{
    // Log request details
    Console.WriteLine($"Request: {context.Request.Path}");
    await next.Invoke();
});

app.Run();

async Task SeedRolesAndAdminUserAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Define roles
    var roles = new[] { "Admin", "Editor", "BasicUser" };

    // Create roles if they don't exist
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Optionally, create a default admin user
    var adminEmail = "admin@spacetheme.com";
    var adminPassword = "Admin@1234";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}