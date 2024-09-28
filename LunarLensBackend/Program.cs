using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using LunarLensBackend.Database;
using Microsoft.EntityFrameworkCore;

var bld = WebApplication.CreateBuilder();
Env.Load();
bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument();

bld.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

bld.Services.AddAuthenticationJwtBearer(x => x.SigningKey = jwtSecret);
bld.Services.AddAuthorization();

bld.Services.AddDbContextFactory<Context>(options =>
{
    options.UseNpgsql("User ID=postgres; Password=1324; Database=lunarlens; Server=localhost; Port=5432; Include Error Detail=true;");
});

var app = bld.Build();

app.UseFastEndpoints();
app.UseCors("AllowLocalhost"); // Enable CORS with the specified policy
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerGen();

app.Run();