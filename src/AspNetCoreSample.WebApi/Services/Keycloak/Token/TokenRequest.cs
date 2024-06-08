using FluentValidation;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Token;

public class TokenRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.UserName).NotNull().NotEmpty();
        RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(4);
    }
}
