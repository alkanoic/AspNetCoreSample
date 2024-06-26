using FluentValidation;

namespace AspNetCoreSample.WebApi.Services.Keycloak.Token;

public class UpdateTokenRequest
{
    public required string RefreshToken { get; set; }
}

public class UpdateTokenRequestValidator : AbstractValidator<UpdateTokenRequest>
{
    public UpdateTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotNull().NotEmpty();
    }
}
