using AspNetCoreSample.WebApi.Models.Keycloak;

namespace AspNetCoreSample.WebApi.Test;

public sealed class KeycloakValidatorTest
{
    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void FetchUserInputValidator_RequiresUsername()
    {
        var validator = new FetchUserInputValidator();

        var result = validator.Validate(new FetchUserInput { Username = "" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void FetchUserInputValidator_PassesWithUsername()
    {
        var validator = new FetchUserInputValidator();

        var result = validator.Validate(new FetchUserInput { Username = "user" });

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void CreateUserInputValidator_RequiresUsernameAndPassword()
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

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void CreateUserInputValidator_PasswordMinLength()
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

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void CreateUserInputValidator_InvalidEmail()
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

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void CreateUserInputValidator_PassesWithValidData()
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

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void UpdateUserInputValidator_RequiresUserId()
    {
        var validator = new UpdateUserInputValidator();

        var result = validator.Validate(new UpdateUserInput { UserId = "" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void UpdateUserInputValidator_PassesWithUserId()
    {
        var validator = new UpdateUserInputValidator();

        var result = validator.Validate(new UpdateUserInput { UserId = "user-id" });

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void ChangePasswordInputValidator_RequiresUserIdAndPassword()
    {
        var validator = new ChangePasswordInputValidator();

        var result = validator.Validate(new ChangePasswordInput { UserId = "", Password = "" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void ChangePasswordInputValidator_PasswordMinLength()
    {
        var validator = new ChangePasswordInputValidator();

        var result = validator.Validate(new ChangePasswordInput { UserId = "user-id", Password = "1234567" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void ChangePasswordInputValidator_PassesWithValidData()
    {
        var validator = new ChangePasswordInputValidator();

        var result = validator.Validate(new ChangePasswordInput { UserId = "user-id", Password = "12345678" });

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void DeleteUserInputValidator_RequiresUserId()
    {
        var validator = new DeleteUserInputValidator();

        var result = validator.Validate(new DeleteUserInput { UserId = "" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void DeleteUserInputValidator_PassesWithUserId()
    {
        var validator = new DeleteUserInputValidator();

        var result = validator.Validate(new DeleteUserInput { UserId = "user-id" });

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void AddUserRoleMappingInputValidator_RequiresUserIdAndDetails()
    {
        var validator = new AddUserRoleMappingInputValidator();

        var result = validator.Validate(new AddUserRoleMappingInput { UserId = "", AddUserRoleMappingInputDetails = null });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void AddUserRoleMappingInputValidator_RequiresAtLeastOneDetail()
    {
        var validator = new AddUserRoleMappingInputValidator();

        var result = validator.Validate(new AddUserRoleMappingInput
        {
            UserId = "user-id",
            AddUserRoleMappingInputDetails = new List<AddUserRoleMappingInputDetail>()
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void AddUserRoleMappingInputValidator_PassesWithValidData()
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

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void AddUserClientRoleMappingInputValidator_RequiresClientUuid()
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

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void AddUserClientRoleMappingInputValidator_PassesWithValidData()
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

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void TokenRequestValidator_RequiresUserNameAndPassword()
    {
        var validator = new Services.Keycloak.Token.TokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.TokenRequest { UserName = "", Password = "" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void TokenRequestValidator_PasswordMinLength()
    {
        var validator = new Services.Keycloak.Token.TokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.TokenRequest { UserName = "user", Password = "123" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void TokenRequestValidator_PassesWithValidData()
    {
        var validator = new Services.Keycloak.Token.TokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.TokenRequest { UserName = "user", Password = "1234" });

        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void RevokeTokenRequestValidator_RequiresRefreshToken()
    {
        var validator = new Services.Keycloak.Token.RevokeTokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.RevokeTokenRequest { RefreshToken = "" });

        Assert.False(result.IsValid);
    }

    [Fact]
    [Trait("Category", nameof(KeycloakValidatorTest))]
    public void UpdateTokenRequestValidator_RequiresRefreshToken()
    {
        var validator = new Services.Keycloak.Token.UpdateTokenRequestValidator();

        var result = validator.Validate(new Services.Keycloak.Token.UpdateTokenRequest { RefreshToken = "" });

        Assert.False(result.IsValid);
    }
}
