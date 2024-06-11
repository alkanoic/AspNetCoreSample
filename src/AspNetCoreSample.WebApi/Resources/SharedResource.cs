using Microsoft.Extensions.Localization;

namespace AspNetCoreSample.WebApi.Resources;

public interface ISharedResource
{
    string UserIdRequired { get; }
    string UserIdLength { get; }
    string RoleIdRequired { get; }
    string RoleIdLength { get; }
    string RoleNameRequired { get; }
    string RoleNameLength { get; }
}

public class SharedResource : ISharedResource
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SharedResource(IStringLocalizer<SharedResource> localizer) =>
        _localizer = localizer;

    public string UserIdRequired => GetString(nameof(UserIdRequired));
    public string UserIdLength => GetString(nameof(UserIdLength));
    public string RoleIdRequired => GetString(nameof(RoleIdRequired));
    public string RoleIdLength => GetString(nameof(RoleIdLength));
    public string RoleNameRequired => GetString(nameof(RoleNameRequired));
    public string RoleNameLength => GetString(nameof(RoleNameLength));

    private string GetString(string name) =>
        _localizer[name];
}
