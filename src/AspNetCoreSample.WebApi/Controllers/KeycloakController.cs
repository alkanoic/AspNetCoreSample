using AspNetCoreSample.WebApi.Models;
using AspNetCoreSample.WebApi.Models.Keycloak;
using AspNetCoreSample.WebApi.Services.Keycloak.Admin;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeycloakController : ControllerBase
{
    private readonly ILogger<KeycloakController> _logger;
    private readonly IKeycloakService _keyclaokService;
    private readonly IValidator<FetchUserInput> _fetchUserInputValidator;
    private readonly IValidator<CreateUserInput> _createUserInputValidator;
    private readonly IValidator<UpdateUserInput> _updateUserInputValidator;
    private readonly IValidator<ChangePasswordInput> _changePasswordInputValidator;
    private readonly IValidator<ResetPasswordByEmailInput> _resetPasswordByEmailInputValidator;
    private readonly IValidator<DeleteUserInput> _deleteUserInputValidator;
    private readonly IValidator<FetchUserRoleMappingsInput> _fetchUserRoleMappingsInputValidator;
    private readonly IValidator<AddUserRoleMappingInput> _addUserRoleMappingInputValidator;
    private readonly IValidator<DeleteUserRoleMappingInput> _deleteUserRoleMappingInputValidator;

    public KeycloakController(ILogger<KeycloakController> logger,
        IKeycloakService keycloakService,
        IValidator<FetchUserInput> fetchUserInputValidator,
        IValidator<CreateUserInput> createUserInputValidator,
        IValidator<UpdateUserInput> updateUserInputValidator,
        IValidator<ChangePasswordInput> changePasswordInputValidator,
        IValidator<ResetPasswordByEmailInput> resetPasswordByEmailInputValidator,
        IValidator<DeleteUserInput> deleteUserInputValidator,
        IValidator<FetchUserRoleMappingsInput> fetchUserRoleMappingsInputValidator,
        IValidator<AddUserRoleMappingInput> addUserRoleMappingInputValidator,
        IValidator<DeleteUserRoleMappingInput> deleteUserRoleMappingInputValidator)
    {
        _logger = logger;
        _keyclaokService = keycloakService;
        _fetchUserInputValidator = fetchUserInputValidator;
        _createUserInputValidator = createUserInputValidator;
        _updateUserInputValidator = updateUserInputValidator;
        _changePasswordInputValidator = changePasswordInputValidator;
        _resetPasswordByEmailInputValidator = resetPasswordByEmailInputValidator;
        _deleteUserInputValidator = deleteUserInputValidator;
        _fetchUserRoleMappingsInputValidator = fetchUserRoleMappingsInputValidator;
        _addUserRoleMappingInputValidator = addUserRoleMappingInputValidator;
        _deleteUserRoleMappingInputValidator = deleteUserRoleMappingInputValidator;
    }

    private async ValueTask<IActionResult> CommonValidationResponse<T>(T input, IValidator<T> validator, Func<ValueTask<IActionResult>> func)
    {
        try
        {
            var result = await validator.ValidateAsync(input);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }

            return await func();
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// ユーザー情報を取得
    /// </summary>
    /// <param name="input">ユーザー名</param>
    [HttpPost("FetchUser")]
    public async ValueTask<IActionResult> FetchUser(FetchUserInput input)
    {
        return await CommonValidationResponse(input, _fetchUserInputValidator, async () =>
        {
            var request = new FetchUserRequest()
            {
                Username = input.Username,
            };
            var response = await _keyclaokService.FetchUserAsync(request);
            return Ok(response);
        });
    }

    /// <summary>
    /// ユーザーを作成する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    [HttpPost("CreateUser")]
    public async ValueTask<IActionResult> CreateUser(CreateUserInput input)
    {
        return await CommonValidationResponse(input, _createUserInputValidator, async () =>
        {
            var request = new CreateUserRequest()
            {
                Username = input.Username,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                Enabled = true,
                Credentials = new List<Credential> { new(input.Password) }
            };
            var response = await _keyclaokService.CreateUserAsync(request);
            return Ok(response);
        });
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    [HttpPut("UpdateUser")]
    public async ValueTask<IActionResult> UpdateUser(UpdateUserInput input)
    {
        return await CommonValidationResponse(input, _updateUserInputValidator, async () =>
        {
            var request = new UpdateUserRequest()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email
            };
            if (!string.IsNullOrWhiteSpace(input.Password))
            {
                request.Credentials = new List<Credential>() { new Credential(input.Password) };
            }
            await _keyclaokService.UpdateUserAsync(input.UserId, request);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーのパスワードを変更する
    /// </summary>
    /// <param name="input">パスワード情報</param>
    [HttpPut("ChangePassword")]
    public async ValueTask<IActionResult> ChangePassword(ChangePasswordInput input)
    {
        return await CommonValidationResponse(input, _changePasswordInputValidator, async () =>
        {
            var request = new ChangePasswordRequest()
            {
                UserId = input.UserId,
                Credential = new Credential(input.Password)
            };
            await _keyclaokService.ChangePasswordAsync(request);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーのパスワードをリセットするためのメールを送信
    /// </summary>
    [HttpPut("ResetPasswordByEmail")]
    public async ValueTask<IActionResult> ResetPasswordByEmail(ResetPasswordByEmailInput input)
    {
        return await CommonValidationResponse(input, _resetPasswordByEmailInputValidator, async () =>
        {
            var request = new ResetPasswordByEmailRequest()
            {
                UserId = input.UserId,
            };
            await _keyclaokService.ResetPasswordByEmailAsync(request);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーを削除する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    [HttpDelete("DeleteUser")]
    public async ValueTask<IActionResult> DeleteUser(DeleteUserInput input)
    {
        return await CommonValidationResponse(input, _deleteUserInputValidator, async () =>
        {
            var request = new DeleteUserRequest()
            {
                UserId = input.UserId,
            };
            await _keyclaokService.DeleteUserAsync(request);
            return Ok();
        });
    }

    /// <summary>
    /// Realmに登録されているロール一覧を取得
    /// </summary>
    [HttpPost("FetchRoles")]
    public async ValueTask<IActionResult> FetchRoles()
    {
        try
        {
            return Ok(await _keyclaokService.FetchRolesAsync());
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// ユーザーに登録されているロール一覧を取得
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    [HttpPost("FtechUserRoleMappings")]
    public async ValueTask<IActionResult> FetchUserRoleMappings(FetchUserRoleMappingsInput input)
    {
        return await CommonValidationResponse(input, _fetchUserRoleMappingsInputValidator, async () =>
        {
            var request = new FetchUserRoleMappingsRequest()
            {
                UserId = input.UserId,
            };
            var result = await _keyclaokService.FetchUserRoleMappingsAsync(request);
            return Ok(result);
        });
    }

    /// <summary>
    /// ユーザーにロールをアタッチする
    /// </summary>
    /// <param name="input">ユーザーとロール情報</param>
    [HttpPost("AddUserRoleMapping")]
    public async ValueTask<IActionResult> AddUserRoleMapping(AddUserRoleMappingInput input)
    {
        return await CommonValidationResponse(input, _addUserRoleMappingInputValidator, async () =>
        {
            var request = new AddUserRoleMappingsRequest()
            {
                Id = input.RoleId
            };
            await _keyclaokService.AddUserRoleMappingAsync(input.UserId, request);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーからロールをデタッチする
    /// </summary>
    /// <param name="input">ユーザーとロール情報</param>
    [HttpDelete("DeleteUserRoleMapping")]
    public async ValueTask<IActionResult> DeleteUserRoleMapping(DeleteUserRoleMappingInput input)
    {
        return await CommonValidationResponse(input, _deleteUserRoleMappingInputValidator, async () =>
        {
            var request = new DeleteUserRoleMappingsRequest()
            {
                Id = input.RoleId
            };
            await _keyclaokService.DeleteUserRoleMappingAsync(input.UserId, request);
            return Ok();
        });
    }
}
