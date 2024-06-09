using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class FetchUserInput
{
    public required string Username { get; set; }
}

public class FetchUserInputValidator : AbstractValidator<FetchUserInput>
{
    public FetchUserInputValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
    }
}
