using System.Collections.Concurrent;

using AspNetCoreSample.WebApi.Options;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AspNetCoreSample.WebApi;

public class PolicyService
{
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _policies;
    private PolicyOptions _policyOptions;
    private DateTime _beforeRefreshDateTime = DateTime.UtcNow;

    public PolicyService(ConcurrentDictionary<string, AuthorizationPolicy> policies,
                        IOptionsMonitor<PolicyOptions> policyOptions)
    {
        _policies = policies;
        _policyOptions = policyOptions.CurrentValue;
        policyOptions.OnChange(settings =>
        {
            _policyOptions = settings;
        });
    }

    /// <summary>
    /// 強制的にポリシーキャッシュをクリアする。
    /// </summary>
    public void RefreshPolicies()
    {
        _policies.Clear();
        _beforeRefreshDateTime = DateTime.UtcNow;
    }

    /// <summary>
    /// 一定のTimeSpanが経過したときにポリシーキャッシュをクリアする。
    /// </summary>
    public void RefreshPoliciesByTimeSpan()
    {
        var diff = DateTime.UtcNow - _beforeRefreshDateTime;
        if (diff > _policyOptions.RefreshPolicyTimeSpan)
        {
            RefreshPolicies();
        }
    }
}
