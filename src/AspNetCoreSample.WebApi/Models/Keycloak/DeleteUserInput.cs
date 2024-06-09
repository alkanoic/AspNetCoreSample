using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class DeleteUserInput
{
    public required string UserId { get; set; }
}

public class DeleteUserInputValidator : AbstractValidator<DeleteUserInput>
{
    public DeleteUserInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
