using System.Text.Json.Serialization;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class UpdateUserRequest
{
    /// <summary>
    /// 名前
    /// </summary>
    public string? FirstName { get; set; }
    /// <summary>
    /// 性別
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// メールアドレス
    /// </summary>
    public string? Email { get; set; }
    public List<Credential>? Credentials { get; set; }
}
