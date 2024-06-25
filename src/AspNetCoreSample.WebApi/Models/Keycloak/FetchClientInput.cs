using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class FetchClientInput
{
    public required string ClientId { get; set; }
}

public class FetchClientInputValidator : AbstractValidator<FetchClientInput>
{
    public FetchClientInputValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
    }
}
