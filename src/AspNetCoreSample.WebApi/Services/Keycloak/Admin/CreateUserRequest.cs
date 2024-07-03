
namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

public class CreateUserRequest
{
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
    public required string FirstName { get; set; }
    /// <summary>
    /// 性別
    /// </summary>
    public required string LastName { get; set; }
    /// <summary>
    /// メールアドレス
    /// </summary>
    public required string Email { get; set; }
    public required List<Credential> Credentials { get; set; }
    /// <summary>
    /// ユーザー属性
    /// </summary>
    public Dictionary<string, List<string>>? Attributes { get; set; }
}
