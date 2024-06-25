using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class DeleteUserRoleMappingInput
{
    public required string UserId { get; set; }
    public List<DeleteUserRoleMappingInputDetail>? DeleteUserRoleMappingInputDetails { get; set; }
}

public class DeleteUserClientRoleMappingInput
{
    public required string UserId { get; set; }
    public required string ClientUuid { get; set; }
    public List<DeleteUserRoleMappingInputDetail>? DeleteUserRoleMappingInputDetails { get; set; }
}

public class DeleteUserRoleMappingInputDetail
{
    public required string RoleId { get; set; }
    public required string RoleName { get; set; }
}

public class DeleteUserRoleMappingInputValidator : AbstractValidator<DeleteUserRoleMappingInput>
{
    public DeleteUserRoleMappingInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DeleteUserRoleMappingInputDetails).NotNull().Must(x => x?.Count != 0).WithMessage("削除するRoleは1つ以上をしてしてください");
        RuleForEach(x => x.DeleteUserRoleMappingInputDetails).SetValidator(new DeleteUserRoleMappingInputDetailValidator());
    }
}

public class DeleteUserClientRoleMappingInputValidator : AbstractValidator<DeleteUserClientRoleMappingInput>
{
    public DeleteUserClientRoleMappingInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ClientUuid).NotEmpty();
        RuleFor(x => x.DeleteUserRoleMappingInputDetails).NotNull().Must(x => x?.Count != 0).WithMessage("削除するRoleは1つ以上をしてしてください");
        RuleForEach(x => x.DeleteUserRoleMappingInputDetails).SetValidator(new DeleteUserRoleMappingInputDetailValidator());
    }
}

public class DeleteUserRoleMappingInputDetailValidator : AbstractValidator<DeleteUserRoleMappingInputDetail>
{
    public DeleteUserRoleMappingInputDetailValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty();
    }
}
