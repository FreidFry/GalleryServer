using Microsoft.Extensions.FileProviders;

namespace Gallery.Server.Core.Extencions.LoadModules
{
    internal static class UseDefaultStaticFilesExtencions
    {
        internal static WebApplication UseBasicStaticFiles(this WebApplication app)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Data", "default")),
                RequestPath = "/default",
                ServeUnknownFileTypes = true
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), app.Configuration.GetValue<string>("FileStorage:UserDataBasePath"))),
                RequestPath = "/images",
                ServeUnknownFileTypes = true
            });
#pragma warning restore CS8604 // Possible null reference argument.
            return app;
        }
    }
}
