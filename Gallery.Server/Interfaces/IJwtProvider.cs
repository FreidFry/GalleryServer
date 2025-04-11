using Gallery.Server.Models.User;

namespace Gallery.Server.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(UserModel user);
    }
}