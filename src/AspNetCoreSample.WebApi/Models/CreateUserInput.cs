using FluentValidation;

namespace AspNetCoreSample.WebApi.Models;

public class CreateUserInput
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email).EmailAddress();
        });
    }
}
