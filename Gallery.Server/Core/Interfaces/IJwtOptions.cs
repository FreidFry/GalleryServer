namespace Gallery.Server.Core.Interfaces
{
    public interface IJwtOptions
    {
        public string SecretKey { get; set; }
        public double ExpiresDays { get; set; }
    }
}
