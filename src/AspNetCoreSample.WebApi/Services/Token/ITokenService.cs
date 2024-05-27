namespace AspNetCoreSample.WebApi.Services.Token;

public interface ITokenService
{
    ValueTask<TokenResponse> GetTokenAsync(TokenRequest tokenRequest);

    ValueTask<TokenResponse> UpdateTokenAsync(UpdateTokenRequest updateTokenRequest);
}
