using dotenv.net;
using Gallery.Server.Core.Interfaces;
using Gallery.Server.Core.Services;
using Gallery.Server.Features.Image.Services;
using Gallery.Server.Features.Profile.Services;
using Gallery.Server.Features.User.Services;
using Gallery.Server.Infrastructure.Persistence.db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = true;
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = ["text/plain", "text/css", "application/javascript", "application/json", "image/svg+xml"];
});

DotEnv.Load(new DotEnvOptions(envFilePaths: ["../settings.env"]));
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.HttpContext.Request.Cookies["jwt"];
                if (token != null)
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // HTTP
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.UseHttps(); // важно
    });
});
builder.Services.AddHttpsRedirection(builder =>
{
    builder.HttpsPort = 443;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("https://localhost:24815")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();

        });
});
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

builder.Services.Configure<JwtOptions>(options =>
{
    options.SecretKey = jwtKey
                        ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");
    options.ExpiresDays = double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRES_DAYS")
                        ?? throw new InvalidOperationException("JWT_EXPIRES_DAYS is not configured."));
});

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowAll");

if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Data/default")))
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Data/default"));
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Data/default")),
    RequestPath = "/default"
});

if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Data/UsersData")))
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Data/UsersData"));
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Data/UsersData")),
    RequestPath = "/images"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
