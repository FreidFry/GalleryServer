using dotenv.net;
using Gallery.Server.Data.db;
using Gallery.Server.Interfaces;
using Gallery.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotEnv.Load();
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("JWT_SECRET_KEY is not set in environment variables");

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

builder.Services.AddDbContext<UsersDbContext>(options =>
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

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "./Data/settings.env" }));
builder.Services.Configure<JwtOptions>(options =>
{
    options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                        ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");
    options.ExpiresDays = double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRES_DAYS")
                        ?? throw new InvalidOperationException("JWT_EXPIRES_DAYS is not configured."));
});

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowAll");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Data/default")),
    RequestPath = "/default"
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
