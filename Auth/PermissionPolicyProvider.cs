using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace OtlobLap.Auth
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider  FallBackPolicyProvider { get; }
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallBackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => await FallBackPolicyProvider.GetDefaultPolicyAsync();

        public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => await FallBackPolicyProvider.GetFallbackPolicyAsync();

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));
                return  await Task.FromResult(policy.Build());
            }
            return await FallBackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
