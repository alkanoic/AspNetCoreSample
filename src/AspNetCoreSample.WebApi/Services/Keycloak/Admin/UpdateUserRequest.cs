using System.Text.Json.Serialization;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class UpdateUserRequest : BaseRequest
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
    /// <summary>
    /// ユーザー属性：更新時はすべて送信すること
    /// </summary>
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
