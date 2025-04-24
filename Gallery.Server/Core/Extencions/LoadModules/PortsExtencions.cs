using System.Runtime.CompilerServices;

namespace Gallery.Server.Core.Extencions.LoadModules
{
    public static class PortsExtencions
    {
        public static IServiceCollection AddPortConfiguration(this IServiceCollection services,
            IWebHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080);
                options.ListenAnyIP(8081, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
            });
            services.AddHttpsRedirection(builder =>
            {
                builder.HttpsPort = 443;
            });

            return services;
        }
    }
}


