using dotenv.net;
using Gallery.Server.Core.Interfaces;

namespace Gallery.Server.Core.Services
{
    public class JwtOptions : IJwtOptions
    {
        public required string SecretKey { get; set; }
        public double ExpiresDays { get; set; }
        public required string issuer { get; set; }
        public required string audience { get; set; }
    }
}