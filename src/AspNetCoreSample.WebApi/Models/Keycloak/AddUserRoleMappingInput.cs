using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class AddUserRoleMappingInput
{
    public required string UserId { get; set; }
    public List<AddUserRoleMappingInputDetail>? AddUserRoleMappingInputDetails { get; set; }
}

public class AddUserRoleMappingInputDetail
{
    public required string RoleId { get; set; }
    public required string RoleName { get; set; }
}

public class AddUserRoleMappingInputValidator : AbstractValidator<AddUserRoleMappingInput>
{
    public AddUserRoleMappingInputValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AddUserRoleMappingInputDetails).NotNull().Must(x => x?.Count != 0).WithMessage("登録するRoleは1つ以上をしてしてください");
        RuleForEach(x => x.AddUserRoleMappingInputDetails).SetValidator(new AddUserRoleMappingInputDetailValidator());
    }
}

public class AddUserRoleMappingInputDetailValidator : AbstractValidator<AddUserRoleMappingInputDetail>
{
    public AddUserRoleMappingInputDetailValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty();
    }
}
