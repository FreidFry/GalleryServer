using Microsoft.AspNetCore.Authentication.JwtBearer;
using dotenv.net;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Gallery.Server.Core.Configuration.EnvConfigurations;
using Gallery.Server.Core.Services;

namespace Gallery.Server.Core.Extencions.LoadModules
{
    internal static class JwtAutheticationExtencions
    {
        internal static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services)
        {
            var envConfig = new JwtEnvConfig();



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envConfig.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = envConfig.Issuer,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = envConfig.Audience,
                };
            });

            services.Configure<JwtOptions>(options =>
            {
                options.SecretKey = envConfig.SecretKey;
                options.ExpiresDays = envConfig.ExpiresDays;
                options.issuer = envConfig.Issuer;
                options.audience = envConfig.Audience;
            });
            return services;
        }
    }
}
