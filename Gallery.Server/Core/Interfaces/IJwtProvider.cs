using Gallery.Server.Infrastructure.Persistence.Models;

namespace Gallery.Server.Core.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(UserModel user);
    }
}