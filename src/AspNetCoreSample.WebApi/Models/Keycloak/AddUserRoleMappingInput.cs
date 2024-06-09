using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class AddUserRoleMappingInput
{
    public required string UserId { get; set; }
    public required string RoleId { get; set; }
}

public class AddUserRoleMappingInputValidator : AbstractValidator<AddUserRoleMappingInput>
{
    public AddUserRoleMappingInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}
