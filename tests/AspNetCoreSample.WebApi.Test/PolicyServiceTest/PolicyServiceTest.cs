using System.Collections.Concurrent;

using AspNetCoreSample.WebApi.Options;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AspNetCoreSample.WebApi.Test;

public sealed class PolicyServiceTest
{
    private static PolicyService CreateService(ConcurrentDictionary<string, AuthorizationPolicy> policies, PolicyOptions options)
    {
        var monitor = new FakeOptionsMonitor<PolicyOptions>(options);
        return new PolicyService(policies, monitor);
    }
    [Test]
    [Category(nameof(PolicyServiceTest))]
    public async Task RefreshPoliciesClearsDictionary()
    {
        var policies = new ConcurrentDictionary<string, AuthorizationPolicy>();
        policies.TryAdd("test", new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        var service = CreateService(policies, new PolicyOptions());

        service.RefreshPolicies();

        await Assert.That(policies).IsEmpty();
    }
    [Test]
    [Category(nameof(PolicyServiceTest))]
    public async Task RefreshPoliciesByTimeSpan_WhenTimeSpanIsZero_DoesNotClear()
    {
        var policies = new ConcurrentDictionary<string, AuthorizationPolicy>();
        policies.TryAdd("test", new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        var options = new PolicyOptions { RefreshPolicyTimeSpan = TimeSpan.Zero };
        var service = CreateService(policies, options);

        service.RefreshPoliciesByTimeSpan();

        await Assert.That(policies).HasSingleItem();
    }
    [Test]
    [Category(nameof(PolicyServiceTest))]
    public async Task RefreshPoliciesByTimeSpan_WhenTimeSpanIsOneSecond_DoesNotClear()
    {
        var policies = new ConcurrentDictionary<string, AuthorizationPolicy>();
        policies.TryAdd("test", new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        var options = new PolicyOptions { RefreshPolicyTimeSpan = TimeSpan.FromSeconds(1) };
        var service = CreateService(policies, options);

        service.RefreshPoliciesByTimeSpan();

        await Assert.That(policies).HasSingleItem();
    }
    [Test]
    [Category(nameof(PolicyServiceTest))]
    public async Task RefreshPoliciesByTimeSpan_WhenElapsed_DoesNotClearImmediately()
    {
        var policies = new ConcurrentDictionary<string, AuthorizationPolicy>();
        policies.TryAdd("test", new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        var options = new PolicyOptions { RefreshPolicyTimeSpan = TimeSpan.FromMinutes(5) };
        var service = CreateService(policies, options);

        service.RefreshPoliciesByTimeSpan();

        await Assert.That(policies).HasSingleItem();
    }

    private sealed class FakeOptionsMonitor<T> : IOptionsMonitor<T>
    {
        public T CurrentValue { get; }

        public FakeOptionsMonitor(T value)
        {
            CurrentValue = value;
        }

        public T Get(string? name) => CurrentValue;

        public IDisposable? OnChange(Action<T, string?> listener)
        {
            return null;
        }
    }
}
