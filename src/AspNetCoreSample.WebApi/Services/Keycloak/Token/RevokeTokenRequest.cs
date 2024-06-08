using FluentValidation;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Token;

public class RevokeTokenRequest
{
    public required string RefreshToken { get; set; }
}

public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
{
    public RevokeTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
    }
}
