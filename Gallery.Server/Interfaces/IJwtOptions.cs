namespace Gallery.Server.Interfaces
{
    public interface IJwtOptions
    {
        public string SecretKey { get; set; }
        public double ExpiresDays { get; set; }
    }
}
