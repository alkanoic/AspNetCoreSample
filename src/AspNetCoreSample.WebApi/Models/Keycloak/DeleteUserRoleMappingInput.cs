using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class DeleteUserRoleMappingInput
{
    public required string UserId { get; set; }
    public required string RoleId { get; set; }
}

public class DeleteUserRoleMappingInputValidator : AbstractValidator<DeleteUserRoleMappingInput>
{
    public DeleteUserRoleMappingInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}
