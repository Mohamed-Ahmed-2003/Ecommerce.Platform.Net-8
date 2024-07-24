using Microsoft.AspNetCore.Authorization;

namespace OtlobLap.Auth
{
    public class PermissionRequirement (string permission) : IAuthorizationRequirement
    {
        public string Permission { get; private set; } = permission;
    }
}
