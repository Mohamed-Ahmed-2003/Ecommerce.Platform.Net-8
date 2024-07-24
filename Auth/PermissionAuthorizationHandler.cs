using Microsoft.AspNetCore.Authorization;

namespace OtlobLap.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User is null) return;

            var permissions = context.User.Claims.Where(c => c.Type == "Permission"
            && c.Value == requirement.Permission
            && c.Issuer == "LOCAL AUTHORITY"
            );
           if (permissions.Any())
            {
                context.Succeed(requirement); return;
            }
        }
    }
}
