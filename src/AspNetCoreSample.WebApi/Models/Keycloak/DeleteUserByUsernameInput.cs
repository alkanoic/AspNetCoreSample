using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class DeleteUserByUsernameInput
{
    public required string Username { get; set; }
}

public class DeleteUserByUsernameInputValidator : AbstractValidator<DeleteUserByUsernameInput>
{
    public DeleteUserByUsernameInputValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
    }
}
