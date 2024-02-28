namespace AspNetCoreSample.WebApi.Services.Token;

public class TokenRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
