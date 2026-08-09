using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;

using AspNetCoreSample.DataModel.Models;
using AspNetCoreSample.WebApi.Options;

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
    private PolicyOptions _policyOptions;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options,
                                            IServiceProvider serviceProvider,
                                            ConcurrentDictionary<string, AuthorizationPolicy> policies,
                                            PolicyService policyService,
                                            IOptionsMonitor<PolicyOptions> policyOptions)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        _serviceProvider = serviceProvider;
        _policies = policies;
        _policyService = policyService;
        _policyOptions = policyOptions.CurrentValue;
        policyOptions.OnChange(settings =>
        {
            _policyOptions = settings;
        });
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
                    .RequireAssertion(handler =>
                    {
                        if (string.IsNullOrEmpty(_policyOptions.ClientRoleName))
                        {
                            // Realm-Roleを使用する場合
                            var json = handler.User.FindFirstValue("realm_access");
                            if (json is null) return false;
                            var realmRoles = JsonSerializer.Deserialize<ClientRoles>(json, _jsonSerializerOptions);
                            if (realmRoles is null || realmRoles.Roles is null) return false;
                            return roles.Intersect(realmRoles.Roles).Any();
                        }
                        else
                        {
                            // Client-Roleを使用する場合
                            var json = handler.User.FindFirstValue("resource_access");
                            if (json is null) return false;
                            var rolesDictionary = JsonSerializer.Deserialize<Dictionary<string, ClientRoles>>(json, _jsonSerializerOptions);
                            if (rolesDictionary is null) return false;
                            if (rolesDictionary.TryGetValue(_policyOptions.ClientRoleName, out var roleValue))
                            {
                                if (roleValue.Roles is null) return false;
                                return roles.Intersect(roleValue.Roles).Any();
                            }
                        }
                        return false;
                    })
                    .Build();

                _policies.TryAdd(policyName, policy);
            }
        }

        return policy ?? await _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    private sealed class ClientRoles
    {
        public List<string>? Roles { get; set; }
    }
}
