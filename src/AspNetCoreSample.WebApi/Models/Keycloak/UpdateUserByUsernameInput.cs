using FluentValidation;

namespace AspNetCoreSample.WebApi.Models.Keycloak;

public class UpdateUserByUsernameInput
{
    public required string Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    /// <summary>
    /// 更新時はすべて送信すること
    /// </summary>
    public Dictionary<string, List<string>>? Attributes { get; set; }
}

public class UpdateUsernameInputValidator : AbstractValidator<UpdateUserByUsernameInput>
{
    public UpdateUsernameInputValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
        {
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        });
        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email).EmailAddress();
        });
    }
}
