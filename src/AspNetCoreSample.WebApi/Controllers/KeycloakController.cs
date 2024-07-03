using AspNetCoreSample.WebApi.Models;
using AspNetCoreSample.WebApi.Models.Keycloak;
using AspNetCoreSample.WebApi.Services.Keycloak.Admin;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeycloakController(ILogger<KeycloakController> logger,
                                IKeycloakService keycloakService,
                                IValidator<FetchUserInput> fetchUserInputValidator,
                                IValidator<CreateUserInput> createUserInputValidator,
                                IValidator<UpdateUserInput> updateUserInputValidator,
                                IValidator<ChangePasswordInput> changePasswordInputValidator,
                                IValidator<ResetPasswordByEmailInput> resetPasswordByEmailInputValidator,
                                IValidator<DeleteUserInput> deleteUserInputValidator,
                                IValidator<FetchUserRoleMappingsInput> fetchUserRoleMappingsInputValidator,
                                IValidator<AddUserRoleMappingInput> addUserRoleMappingInputValidator,
                                IValidator<DeleteUserRoleMappingInput> deleteUserRoleMappingInputValidator,
                                IValidator<FetchClientInput> fetchClientInputValidator,
                                IValidator<FetchClientRolesInput> fetchClientRolesInputValidator,
                                IValidator<FetchUserClientRolesInput> fetchUserClientRolesInputValidator,
                                IValidator<AddUserClientRoleMappingInput> addUserClientRoleMappingInputValidator,
                                IValidator<DeleteUserClientRoleMappingInput> deleteUserClientRoleMappingInputValidator) : ControllerBase
{
    private readonly ILogger<KeycloakController> _logger = logger;
    private readonly IKeycloakService _keycloakService = keycloakService;
    private readonly IValidator<FetchUserInput> _fetchUserInputValidator = fetchUserInputValidator;
    private readonly IValidator<CreateUserInput> _createUserInputValidator = createUserInputValidator;
    private readonly IValidator<UpdateUserInput> _updateUserInputValidator = updateUserInputValidator;
    private readonly IValidator<ChangePasswordInput> _changePasswordInputValidator = changePasswordInputValidator;
    private readonly IValidator<ResetPasswordByEmailInput> _resetPasswordByEmailInputValidator = resetPasswordByEmailInputValidator;
    private readonly IValidator<DeleteUserInput> _deleteUserInputValidator = deleteUserInputValidator;
    private readonly IValidator<FetchUserRoleMappingsInput> _fetchUserRoleMappingsInputValidator = fetchUserRoleMappingsInputValidator;
    private readonly IValidator<AddUserRoleMappingInput> _addUserRoleMappingInputValidator = addUserRoleMappingInputValidator;
    private readonly IValidator<DeleteUserRoleMappingInput> _deleteUserRoleMappingInputValidator = deleteUserRoleMappingInputValidator;
    private readonly IValidator<FetchClientInput> _fetchClientInputValidator = fetchClientInputValidator;
    private readonly IValidator<FetchClientRolesInput> _fetchClientRolesInputValidator = fetchClientRolesInputValidator;
    private readonly IValidator<FetchUserClientRolesInput> _fetchUserClientRolesInputValidator = fetchUserClientRolesInputValidator;
    private readonly IValidator<AddUserClientRoleMappingInput> _addUserClientRoleMappingInputValidator = addUserClientRoleMappingInputValidator;
    private readonly IValidator<DeleteUserClientRoleMappingInput> _deleteUserClientRoleMappingInputValidator = deleteUserClientRoleMappingInputValidator;

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

    private string GetAccessTokenByHeader()
    {
        return HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
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
                AccessToken = GetAccessTokenByHeader(),
                Username = input.Username,
            };
            var response = await _keycloakService.FetchUserAsync(request);
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
                Credentials = new List<Credential> { new(input.Password) },
                Attributes = input.Attributes
            };
            var response = await _keycloakService.CreateUserAsync(request);
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
                Email = input.Email,
                Attributes = input.Attributes
            };
            if (!string.IsNullOrWhiteSpace(input.Password))
            {
                request.Credentials = new List<Credential>() { new Credential(input.Password) };
            }
            await _keycloakService.UpdateUserAsync(input.UserId, request);
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
            await _keycloakService.ChangePasswordAsync(request);
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
            await _keycloakService.ResetPasswordByEmailAsync(request);
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
            await _keycloakService.DeleteUserAsync(request);
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
            return Ok(await _keycloakService.FetchRolesAsync());
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
            var result = await _keycloakService.FetchUserRoleMappingsAsync(request);
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
            var request = new List<AddUserRoleMappingsRequest>();
            foreach (var a in input.AddUserRoleMappingInputDetails ?? new List<AddUserRoleMappingInputDetail>())
            {
                request.Add(new AddUserRoleMappingsRequest()
                {
                    Id = a.RoleId,
                    Name = a.RoleName
                });
            }
            await _keycloakService.AddUserRoleMappingAsync(input.UserId, request);
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
            var request = new List<DeleteUserRoleMappingsRequest>();
            foreach (var a in input.DeleteUserRoleMappingInputDetails ?? new List<DeleteUserRoleMappingInputDetail>())
            {
                request.Add(new DeleteUserRoleMappingsRequest()
                {
                    Id = a.RoleId,
                    Name = a.RoleName
                });
            }
            await _keycloakService.DeleteUserRoleMappingAsync(input.UserId, request);
            return Ok();
        });
    }

    /// <summary>
    /// Client一覧を取得する
    /// </summary>
    [HttpPost("FetchClients")]
    public async ValueTask<IActionResult> FetchClients()
    {
        try
        {
            var request = new FetchClientsRequest()
            {
                AccessToken = GetAccessTokenByHeader()
            };
            return Ok(await _keycloakService.FetchClientsAsync(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// ClientをClientIdで検索して取得する
    /// </summary>
    [HttpPost("FetchClient")]
    public async ValueTask<IActionResult> FetchClient(FetchClientInput input)
    {
        return await CommonValidationResponse(input, _fetchClientInputValidator, async () =>
        {
            var request = new FetchClientRequest()
            {
                AccessToken = GetAccessTokenByHeader(),
                ClientId = input.ClientId
            };
            return Ok(await _keycloakService.FetchClientAsync(request));
        });
    }

    /// <summary>
    /// Client-Roleの一覧を取得する
    /// </summary>
    [HttpPost("FetchClientRoles")]
    public async ValueTask<IActionResult> FetchClientRoles(FetchClientRolesInput input)
    {
        return await CommonValidationResponse(input, _fetchClientRolesInputValidator, async () =>
        {
            var request = new FetchClientRolesRequest()
            {
                AccessToken = GetAccessTokenByHeader(),
                ClientUuid = input.ClientUuid
            };
            return Ok(await _keycloakService.FetchClientRolesAsync(request));
        });
    }

    /// <summary>
    /// ユーザーに紐づくClient-Roleを取得する
    /// </summary>
    [HttpPost("FetchUserClientRoles")]
    public async ValueTask<IActionResult> FetchUserClientRoles(FetchUserClientRolesInput input)
    {
        return await CommonValidationResponse(input, _fetchUserClientRolesInputValidator, async () =>
        {
            var request = new FetchUserClientRolesRequest()
            {
                AccessToken = GetAccessTokenByHeader(),
                UserId = input.UserId,
                ClientUuid = input.ClientUuid
            };
            return Ok(await _keycloakService.FetchUserClientRolesAsync(request));
        });
    }

    /// <summary>
    /// ユーザーにClient-Roleをアタッチする
    /// </summary>
    [HttpPost("AddUserClientRoleMappings")]
    public async ValueTask<IActionResult> AddUserClientRoleMappings(AddUserClientRoleMappingInput input)
    {
        return await CommonValidationResponse(input, _addUserClientRoleMappingInputValidator, async () =>
        {
            var request = new List<AddUserRoleMappingsRequest>();
            foreach (var a in input.AddUserRoleMappingInputDetails ?? new List<AddUserRoleMappingInputDetail>())
            {
                request.Add(new AddUserRoleMappingsRequest()
                {
                    Id = a.RoleId,
                    Name = a.RoleName
                });
            }
            await _keycloakService.AddUserClientRoleMappingAsync(input.UserId, input.ClientUuid, request);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーからClient-Roleをデタッチする
    /// </summary>
    [HttpDelete("DeleteUserClientRoleMapping")]
    public async ValueTask<IActionResult> DeleteUserClientRoleMapping(DeleteUserClientRoleMappingInput input)
    {
        return await CommonValidationResponse(input, _deleteUserClientRoleMappingInputValidator, async () =>
        {
            var request = new List<DeleteUserRoleMappingsRequest>();
            foreach (var a in input.DeleteUserRoleMappingInputDetails ?? new List<DeleteUserRoleMappingInputDetail>())
            {
                request.Add(new DeleteUserRoleMappingsRequest()
                {
                    Id = a.RoleId,
                    Name = a.RoleName
                });
            }
            await _keycloakService.DeleteUserClientRoleMappingAsync(input.UserId, input.ClientUuid, request);
            return Ok();
        });
    }
}
