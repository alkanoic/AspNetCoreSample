using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class ChangePasswordInput
{
    public required string UserId { get; set; }
    public required string Password { get; set; }
}

public class ChangePasswordInputValidator : AbstractValidator<ChangePasswordInput>
{
    public ChangePasswordInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
