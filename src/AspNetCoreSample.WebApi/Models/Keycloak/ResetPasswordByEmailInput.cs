using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class ResetPasswordByEmailInput
{
    public required string UserId { get; set; }
}

public class ResetPasswordByEmailInputValidator : AbstractValidator<ResetPasswordByEmailInput>
{
    public ResetPasswordByEmailInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
