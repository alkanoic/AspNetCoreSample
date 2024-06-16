using System.Collections.Concurrent;

using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreSample.WebApi;

public class PolicyService
{
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _policies;

    public PolicyService(ConcurrentDictionary<string, AuthorizationPolicy> policies)
    {
        _policies = policies;
    }

    public void RefreshPolicies()
    {
        _policies.Clear();
    }
}
