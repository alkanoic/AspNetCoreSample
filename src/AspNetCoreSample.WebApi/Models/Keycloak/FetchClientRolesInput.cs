using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class FetchClientRolesInput
{
    public required string ClientUuid { get; set; }
}

public class FetchClientRolesInputValidator : AbstractValidator<FetchClientRolesInput>
{
    public FetchClientRolesInputValidator()
    {
        RuleFor(x => x.ClientUuid).NotEmpty();
    }
}
