using System.Collections.Concurrent;

using AspNetCoreSample.DataModel.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCoreSample.WebApi;

public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _policies;
    private readonly PolicyService _policyService;

    public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options,
                                            IServiceProvider serviceProvider,
                                            ConcurrentDictionary<string, AuthorizationPolicy> policies,
                                            PolicyService policyService)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        _serviceProvider = serviceProvider;
        _policies = policies;
        _policyService = policyService;
    }

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        _policyService.RefreshPoliciesByTimeSpan();
        if (_policies.TryGetValue(policyName, out var policy))
        {
            return policy;
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SampleContext>();
            var roles = await dbContext.RolePolicies.Where(x => x.PolicyName == policyName).Select(x => x.RoleName).ToListAsync();
            if (roles != null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(handler => handler.User.HasClaim(c => roles.Contains(c.Value)))
                    .Build();

                _policies.TryAdd(policyName, policy);
            }
        }

        return policy ?? await _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();
}
