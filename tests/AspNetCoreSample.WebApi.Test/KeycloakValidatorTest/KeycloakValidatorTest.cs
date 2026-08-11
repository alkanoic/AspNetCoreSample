using AspNetCoreSample.WebApi.Models.Keycloak;

namespace AspNetCoreSample.WebApi.Test;

public sealed class KeycloakValidatorTest
{
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task FetchUserInputValidator_RequiresUsername()
    {
        var validator = new FetchUserInputValidator();

        var result = validator.Validate(new FetchUserInput { Username = "" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task FetchUserInputValidator_PassesWithUsername()
    {
        var validator = new FetchUserInputValidator();

        var result = validator.Validate(new FetchUserInput { Username = "user" });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task CreateUserInputValidator_RequiresUsernameAndPassword()
    {
        var validator = new CreateUserInputValidator();

        var result = validator.Validate(new CreateUserInput
        {
            Username = "",
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com",
            Password = ""
        });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task CreateUserInputValidator_PasswordMinLength()
    {
        var validator = new CreateUserInputValidator();

        var result = validator.Validate(new CreateUserInput
        {
            Username = "user",
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com",
            Password = "1234567"
        });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task CreateUserInputValidator_InvalidEmail()
    {
        var validator = new CreateUserInputValidator();

        var result = validator.Validate(new CreateUserInput
        {
            Username = "user",
            FirstName = "First",
            LastName = "Last",
            Email = "not-an-email",
            Password = "12345678"
        });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task CreateUserInputValidator_PassesWithValidData()
    {
        var validator = new CreateUserInputValidator();

        var result = validator.Validate(new CreateUserInput
        {
            Username = "user",
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com",
            Password = "12345678"
        });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task UpdateUserInputValidator_RequiresUserId()
    {
        var validator = new UpdateUserInputValidator();

        var result = validator.Validate(new UpdateUserInput { UserId = "" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task UpdateUserInputValidator_PassesWithUserId()
    {
        var validator = new UpdateUserInputValidator();

        var result = validator.Validate(new UpdateUserInput { UserId = "user-id" });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task ChangePasswordInputValidator_RequiresUserIdAndPassword()
    {
        var validator = new ChangePasswordInputValidator();

        var result = validator.Validate(new ChangePasswordInput { UserId = "", Password = "" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task ChangePasswordInputValidator_PasswordMinLength()
    {
        var validator = new ChangePasswordInputValidator();

        var result = validator.Validate(new ChangePasswordInput { UserId = "user-id", Password = "1234567" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task ChangePasswordInputValidator_PassesWithValidData()
    {
        var validator = new ChangePasswordInputValidator();

        var result = validator.Validate(new ChangePasswordInput { UserId = "user-id", Password = "12345678" });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task DeleteUserInputValidator_RequiresUserId()
    {
        var validator = new DeleteUserInputValidator();

        var result = validator.Validate(new DeleteUserInput { UserId = "" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task DeleteUserInputValidator_PassesWithUserId()
    {
        var validator = new DeleteUserInputValidator();

        var result = validator.Validate(new DeleteUserInput { UserId = "user-id" });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task AddUserRoleMappingInputValidator_RequiresUserIdAndDetails()
    {
        var validator = new AddUserRoleMappingInputValidator();

        var result = validator.Validate(new AddUserRoleMappingInput { UserId = "", AddUserRoleMappingInputDetails = null });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task AddUserRoleMappingInputValidator_RequiresAtLeastOneDetail()
    {
        var validator = new AddUserRoleMappingInputValidator();

        var result = validator.Validate(new AddUserRoleMappingInput
        {
            UserId = "user-id",
            AddUserRoleMappingInputDetails = new List<AddUserRoleMappingInputDetail>()
        });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task AddUserRoleMappingInputValidator_PassesWithValidData()
    {
        var validator = new AddUserRoleMappingInputValidator();

        var result = validator.Validate(new AddUserRoleMappingInput
        {
            UserId = "user-id",
            AddUserRoleMappingInputDetails = new List<AddUserRoleMappingInputDetail>
            {
                new() { RoleId = "role-id", RoleName = "role-name" }
            }
        });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task AddUserClientRoleMappingInputValidator_RequiresClientUuid()
    {
        var validator = new AddUserClientRoleMappingInputValidator();

        var result = validator.Validate(new AddUserClientRoleMappingInput
        {
            UserId = "user-id",
            ClientUuid = "",
            AddUserRoleMappingInputDetails = new List<AddUserRoleMappingInputDetail>
            {
                new() { RoleId = "role-id", RoleName = "role-name" }
            }
        });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task AddUserClientRoleMappingInputValidator_PassesWithValidData()
    {
        var validator = new AddUserClientRoleMappingInputValidator();

        var result = validator.Validate(new AddUserClientRoleMappingInput
        {
            UserId = "user-id",
            ClientUuid = "client-uuid",
            AddUserRoleMappingInputDetails = new List<AddUserRoleMappingInputDetail>
            {
                new() { RoleId = "role-id", RoleName = "role-name" }
            }
        });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task TokenRequestValidator_RequiresUserNameAndPassword()
    {
        var validator = new Services.Keycloak.Token.TokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.TokenRequest { UserName = "", Password = "" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task TokenRequestValidator_PasswordMinLength()
    {
        var validator = new Services.Keycloak.Token.TokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.TokenRequest { UserName = "user", Password = "123" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task TokenRequestValidator_PassesWithValidData()
    {
        var validator = new Services.Keycloak.Token.TokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.TokenRequest { UserName = "user", Password = "1234" });

        await Assert.That(result.IsValid).IsTrue();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task RevokeTokenRequestValidator_RequiresRefreshToken()
    {
        var validator = new Services.Keycloak.Token.RevokeTokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.RevokeTokenRequest { RefreshToken = "" });

        await Assert.That(result.IsValid).IsFalse();
    }
    [Test]
    [Category(nameof(KeycloakValidatorTest))]
    public async Task UpdateTokenRequestValidator_RequiresRefreshToken()
    {
        var validator = new Services.Keycloak.Token.UpdateTokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.UpdateTokenRequest { RefreshToken = "" });

        await Assert.That(result.IsValid).IsFalse();
    }
}
