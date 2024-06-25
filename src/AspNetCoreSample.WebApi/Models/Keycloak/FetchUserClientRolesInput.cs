using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class FetchUserClientRolesInput
{
    public required string UserId { get; set; }
    public required string ClientUuid { get; set; }
}

public class FetchUserClientRolesInputValidator : AbstractValidator<FetchUserClientRolesInput>
{
    public FetchUserClientRolesInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ClientUuid).NotEmpty();
    }
}
