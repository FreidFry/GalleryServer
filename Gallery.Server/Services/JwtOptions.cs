using dotenv.net;
using Gallery.Server.Interfaces;

namespace Gallery.Server.Services
{
    public class JwtOptions : IJwtOptions
    {
        public string SecretKey { get; set; }
        public double ExpiresDays { get; set; }

        //public static IJwtOptions Create(string path)
        //{
        //    DotEnv.Load(options: new DotEnvOptions(envFilePaths: [ string.IsNullOrEmpty(path) ? "./Data/settings.env" : path ]));

        //    string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
        //            ?? throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");
        //    string expiresDaysStr = Environment.GetEnvironmentVariable("JWT_EXPIRES_DAYS")
        //                            ?? throw new InvalidOperationException("JWT_EXPIRES_DAYS is not configured.");
        //    if (!double.TryParse(expiresDaysStr, out double expiresDays))
        //    {
        //        throw new InvalidOperationException("JWT_EXPIRES_DAYS is not a valid number.");
        //    }

        //    return new JwtOptions { ExpiresDays = expiresDays, SecretKey = secretKey };
        //}
    }
}