using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class ResetPasswordByEmailByUsernameInput
{
    public required string Username { get; set; }
}

public class ResetPasswordByEmailByUsernameInputValidator : AbstractValidator<ResetPasswordByEmailByUsernameInput>
{
    public ResetPasswordByEmailByUsernameInputValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
    }
}
