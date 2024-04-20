namespace AspNetCoreSample.WebApi.Services.Token;

public interface ITokenService
{
    ValueTask<TokenResponse> GetTokenAsync(TokenRequest tokenRequest);
}
