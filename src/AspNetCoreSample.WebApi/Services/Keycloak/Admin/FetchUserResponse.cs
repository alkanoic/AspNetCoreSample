
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class FetchUserResponse
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    public required string Id { get; set; }
    /// <summary>
    /// ユーザー名：ログインに使用する
    /// </summary>
    public required string Username { get; set; }
    /// <summary>
    /// ユーザーの有効化：デフォルト有効化
    /// </summary>
    public bool Enabled { get; set; } = true;
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
    /// <summary>
    /// Attributes
    /// </summary>
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
