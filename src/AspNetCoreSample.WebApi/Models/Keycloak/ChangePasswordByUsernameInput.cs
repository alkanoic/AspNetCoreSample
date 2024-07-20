using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class ChangePasswordByUsernameInput
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class ChangePasswordByUsernameInputValidator : AbstractValidator<ChangePasswordByUsernameInput>
{
    public ChangePasswordByUsernameInputValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
