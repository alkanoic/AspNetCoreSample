using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class FetchUserRoleMappingsInput
{
    public required string UserId { get; set; }
}

public class FetchUserRoleMappingsInputValidator : AbstractValidator<FetchUserRoleMappingsInput>
{
    public FetchUserRoleMappingsInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
