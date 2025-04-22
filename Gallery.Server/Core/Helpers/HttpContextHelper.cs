using Gallery.Server.Core.Interfaces;
using System.Security.Claims;

namespace Gallery.Server.Core.Helpers
{
    public class HttpContextHelper : IHttpContextHelper
    {
        public Guid? GetUserId(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return null;

            return userId;
        }

        public bool IsAuthenticated(HttpContext httpContext)
        {
            return httpContext.User.Identity?.IsAuthenticated == true;
        }

        public bool IsOwner(HttpContext httpContext, Guid resourceOwnerId)
        {
            var currentUserId = GetUserId(httpContext);
            return currentUserId.HasValue && currentUserId.Value == resourceOwnerId;
        }

        public bool HasPermission(HttpContext httpContext, Guid resourceOwnerId, bool isPublic)
        {
            if (isPublic)
                return true;

            return IsOwner(httpContext, resourceOwnerId);
        }
    }
}