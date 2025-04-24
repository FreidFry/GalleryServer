using dotenv.net;

namespace Gallery.Server.Core.Configuration.EnvConfigurations
{
    internal class JwtEnvConfig
    {
        internal string Issuer { get; private set; }
        internal string Audience { get; private set; }
        internal string SecretKey { get; private set; }
        internal double ExpiresDays { get; private set; }

        internal JwtEnvConfig(string path)
        {
            var envPath = GetDefaultEnv(Path.Combine(path ?? Directory.GetCurrentDirectory(), "settings.env"));
            DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envPath }));
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER is not configured.");
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE is not configured.");
            SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");
            ExpiresDays = double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRES_DAYS") ?? throw new InvalidOperationException("JWT_EXPIRES_DAYS is not configured."));
        }

        internal JwtEnvConfig()
        {
            var envPath = GetDefaultEnv(Path.Combine(Directory.GetCurrentDirectory(), "settings.env"));
            DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envPath }));
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER is not configured.");
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE is not configured.");
            SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");
            ExpiresDays = double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRES_DAYS") ?? throw new InvalidOperationException("JWT_EXPIRES_DAYS is not configured."));
        }

        private string GetDefaultEnv(string path)
        {
            if (!File.Exists(path)){
                return Path.Combine(Directory.GetCurrentDirectory(), "settings.Example.env");
            }
                return path;
        }
    }
}
