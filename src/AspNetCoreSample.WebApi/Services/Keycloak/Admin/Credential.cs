namespace AspNetCoreSample.WebApi.Services.Keycloak.Admin;

/// <summary>
/// パスワード情報
/// </summary>
public class Credential
{
    public string Type { get; set; }

    /// <summary>
    /// パスワード
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 一時的なパスワードか
    /// </summary>
    public bool Temporary { get; set; }

    public Credential(string password)
    {
        Type = "password";
        Value = password;
        Temporary = false;
    }
}
