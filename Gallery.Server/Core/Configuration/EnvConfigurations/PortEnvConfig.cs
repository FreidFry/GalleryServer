using dotenv.net;
using System.IO;

namespace Gallery.Server.Core.Configuration.EnvConfigurations
{
    public class PortEnvConfig
    {
        public string HttpPort { get; private set; }
        public string HttpsPort { get; private set; }

        public PortEnvConfig(string path)
        {
            var envPath = Path.Combine(path ?? Directory.GetCurrentDirectory(), "settings.env");
            DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envPath }));
            HttpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? throw new InvalidOperationException("HTTP_PORT is not configured.");
            HttpsPort = Environment.GetEnvironmentVariable("HTTPS_PORT") ?? throw new InvalidOperationException("HTTPS_PORT is not configured.");

        }
    }
}
