namespace Gallery.Server.Core.Interfaces
{
    public interface IHttpContextHelper
    {
        Guid? GetUserId(HttpContext httpContext);
        bool HasPermission(HttpContext httpContext, Guid resourceOwnerId, bool isPublic);
        bool IsAuthenticated(HttpContext httpContext);
        bool IsOwner(HttpContext httpContext, Guid resourceOwnerId);
    }
}