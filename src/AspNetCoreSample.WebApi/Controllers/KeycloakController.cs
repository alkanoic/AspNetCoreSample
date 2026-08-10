using AspNetCoreSample.WebApi.Models;
using AspNetCoreSample.WebApi.Models.Keycloak;
using AspNetCoreSample.WebApi.Services.Keycloak.Admin;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class KeycloakController(ILogger<KeycloakController> logger,
                                IKeycloakService keycloakService,
                                IValidator<FetchUserInput> fetchUserInputValidator,
                                IValidator<CreateUserInput> createUserInputValidator,
                                IValidator<UpdateUserInput> updateUserInputValidator,
                                IValidator<UpdateUserByUsernameInput> updateUserByUsernameInputValidator,
                                IValidator<ChangePasswordInput> changePasswordInputValidator,
                                IValidator<ChangePasswordByUsernameInput> changePasswordByUsernameInputValidator,
                                IValidator<ResetPasswordByEmailInput> resetPasswordByEmailInputValidator,
                                IValidator<ResetPasswordByEmailByUsernameInput> resetPasswordByEmailByUsernameInputValidator,
                                IValidator<DeleteUserInput> deleteUserInputValidator,
                                IValidator<DeleteUserByUsernameInput> deleteUserByUsernameInputValidator,
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
    private readonly IValidator<UpdateUserByUsernameInput> _updateUserByUsernameInputValidator = updateUserByUsernameInputValidator;
    private readonly IValidator<ChangePasswordInput> _changePasswordInputValidator = changePasswordInputValidator;
    private readonly IValidator<ChangePasswordByUsernameInput> _changePasswordByUsernameInputValidator = changePasswordByUsernameInputValidator;
    private readonly IValidator<ResetPasswordByEmailInput> _resetPasswordByEmailInputValidator = resetPasswordByEmailInputValidator;
    private readonly IValidator<ResetPasswordByEmailByUsernameInput> _resetPasswordByEmailByUsernameInputValidator = resetPasswordByEmailByUsernameInputValidator;
    private readonly IValidator<DeleteUserInput> _deleteUserInputValidator = deleteUserInputValidator;
    private readonly IValidator<DeleteUserByUsernameInput> _deleteUserByUsernameInputValidator = deleteUserByUsernameInputValidator;
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
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Invalid data in KeycloakController");
            return BadRequest(new WebApiFailResponse("Invalid request data"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in KeycloakController");
            return StatusCode(500, new WebApiFailResponse("Internal server error"));
        }
    }

    private string GetAccessTokenByHeader()
    {
        return HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    }

    /// <summary>
    /// ユーザー情報を取得
    /// </summary>
    /// <param name="username">ユーザー名</param>
    /// <param name="ct">CancellationToken</param>
    [HttpGet("FetchUser")]
    public async ValueTask<IActionResult> FetchUser([FromQuery] string username, CancellationToken ct)
    {
        return await CommonValidationResponse(new FetchUserInput { Username = username }, _fetchUserInputValidator, async () =>
        {
            var request = new FetchUserRequest()
            {
                AccessToken = GetAccessTokenByHeader(),
                Username = username,
            };
            var response = await _keycloakService.FetchUserAsync(request, ct);
            return Ok(response);
        });
    }

    /// <summary>
    /// ユーザーを作成する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    /// <param name="ct">CancellationToken</param>
    [HttpPost("CreateUser")]
    public async ValueTask<IActionResult> CreateUser(CreateUserInput input, CancellationToken ct)
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
            var response = await _keycloakService.CreateUserAsync(request, ct);
            return Ok(response);
        });
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    /// <param name="ct">CancellationToken</param>
    [HttpPut("UpdateUser")]
    public async ValueTask<IActionResult> UpdateUser(UpdateUserInput input, CancellationToken ct)
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
            await _keycloakService.UpdateUserAsync(input.UserId, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// ユーザー名で更新する
    /// </summary>
    [HttpPut("UpdateUserByUsername")]
    public async ValueTask<IActionResult> UpdateUserByUsername(UpdateUserByUsernameInput input, CancellationToken ct)
    {
        return await CommonValidationResponse(input, _updateUserByUsernameInputValidator, async () =>
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
            await _keycloakService.UpdateUserByUsernameAsync(input.Username, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーのパスワードを変更する
    /// </summary>
    /// <param name="input">パスワード情報</param>
    /// <param name="ct">CancellationToken</param>
    [HttpPut("ChangePassword")]
    public async ValueTask<IActionResult> ChangePassword(ChangePasswordInput input, CancellationToken ct)
    {
        return await CommonValidationResponse(input, _changePasswordInputValidator, async () =>
        {
            var request = new ChangePasswordRequest()
            {
                Credential = new Credential(input.Password)
            };
            await _keycloakService.ChangePasswordAsync(input.UserId, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーのパスワードを変更する
    /// </summary>
    /// <param name="input">パスワード情報</param>
    /// <param name="ct">CancellationToken</param>
    [HttpPut("ChangePasswordByUsername")]
    public async ValueTask<IActionResult> ChangePasswordByUsername(ChangePasswordByUsernameInput input, CancellationToken ct)
    {
        return await CommonValidationResponse(input, _changePasswordByUsernameInputValidator, async () =>
        {
            var request = new ChangePasswordRequest()
            {
                Credential = new Credential(input.Password)
            };
            await _keycloakService.ChangePasswordByUsernameAsync(input.Username, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーのパスワードをリセットするためのメールを送信
    /// </summary>
    [HttpPut("ResetPasswordByEmail")]
    public async ValueTask<IActionResult> ResetPasswordByEmail(ResetPasswordByEmailInput input, CancellationToken ct)
    {
        return await CommonValidationResponse(input, _resetPasswordByEmailInputValidator, async () =>
        {
            var request = new ResetPasswordByEmailRequest();
            await _keycloakService.ResetPasswordByEmailAsync(input.UserId, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザー名でユーザーのパスワードをリセットするためのメールを送信
    /// </summary>
    [HttpPut("ResetPasswordByEmailByUsername")]
    public async ValueTask<IActionResult> ResetPasswordByEmailByUsername(ResetPasswordByEmailByUsernameInput input, CancellationToken ct)
    {
        return await CommonValidationResponse(input, _resetPasswordByEmailByUsernameInputValidator, async () =>
        {
            var request = new ResetPasswordByEmailRequest();
            await _keycloakService.ResetPasswordByEmailByUsernameAsync(input.Username, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーを削除する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="ct">CancellationToken</param>
    [HttpDelete("DeleteUser/{userId}")]
    public async ValueTask<IActionResult> DeleteUser(string userId, CancellationToken ct)
    {
        return await CommonValidationResponse(new DeleteUserInput { UserId = userId }, _deleteUserInputValidator, async () =>
        {
            var request = new DeleteUserRequest();
            await _keycloakService.DeleteUserAsync(userId, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザー名でユーザーを削除する
    /// </summary>
    [HttpDelete("DeleteUserByUsername/{username}")]
    public async ValueTask<IActionResult> DeleteUserByUsername(string username, CancellationToken ct)
    {
        return await CommonValidationResponse(new DeleteUserByUsernameInput { Username = username }, _deleteUserByUsernameInputValidator, async () =>
        {
            var request = new DeleteUserRequest();
            await _keycloakService.DeleteUserByUsernameAsync(username, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// Realmに登録されているロール一覧を取得
    /// </summary>
    [HttpGet("FetchRoles")]
    public async ValueTask<IActionResult> FetchRoles(CancellationToken ct)
    {
        try
        {
            return Ok(await _keycloakService.FetchRolesAsync(ct));
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Invalid data fetching roles");
            return BadRequest(new WebApiFailResponse("Invalid request data"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching roles");
            return StatusCode(500, new WebApiFailResponse("Internal server error"));
        }
    }

    /// <summary>
    /// ユーザーに登録されているロール一覧を取得
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="ct">CancellationToken</param>
    [HttpGet("FetchUserRoleMappings")]
    public async ValueTask<IActionResult> FetchUserRoleMappings([FromQuery] string userId, CancellationToken ct)
    {
        return await CommonValidationResponse(new FetchUserRoleMappingsInput { UserId = userId }, _fetchUserRoleMappingsInputValidator, async () =>
        {
            var request = new FetchUserRoleMappingsRequest()
            {
                UserId = userId,
            };
            var result = await _keycloakService.FetchUserRoleMappingsAsync(request, ct);
            return Ok(result);
        });
    }

    /// <summary>
    /// ユーザーにロールをアタッチする
    /// </summary>
    /// <param name="input">ユーザーとロール情報</param>
    /// <param name="ct">CancellationToken</param>
    [HttpPost("AddUserRoleMapping")]
    public async ValueTask<IActionResult> AddUserRoleMapping(AddUserRoleMappingInput input, CancellationToken ct)
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
            await _keycloakService.AddUserRoleMappingAsync(input.UserId, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーからロールをデタッチする
    /// </summary>
    /// <param name="input">ユーザーとロール情報</param>
    /// <param name="ct">CancellationToken</param>
    [HttpDelete("DeleteUserRoleMapping")]
    public async ValueTask<IActionResult> DeleteUserRoleMapping(DeleteUserRoleMappingInput input, CancellationToken ct)
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
            await _keycloakService.DeleteUserRoleMappingAsync(input.UserId, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// Client一覧を取得する
    /// </summary>
    [HttpGet("FetchClients")]
    public async ValueTask<IActionResult> FetchClients(CancellationToken ct)
    {
        try
        {
            var request = new FetchClientsRequest()
            {
                AccessToken = GetAccessTokenByHeader()
            };
            return Ok(await _keycloakService.FetchClientsAsync(request, ct));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Invalid data fetching clients");
            return BadRequest(new WebApiFailResponse("Invalid request data"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching clients");
            return StatusCode(500, new WebApiFailResponse("Internal server error"));
        }
    }

    /// <summary>
    /// ClientをClientIdで検索して取得する
    /// </summary>
    [HttpGet("FetchClient")]
    public async ValueTask<IActionResult> FetchClient([FromQuery] string clientId, CancellationToken ct)
    {
        return await CommonValidationResponse(new FetchClientInput { ClientId = clientId }, _fetchClientInputValidator, async () =>
        {
            var request = new FetchClientRequest()
            {
                AccessToken = GetAccessTokenByHeader(),
                ClientId = clientId
            };
            return Ok(await _keycloakService.FetchClientAsync(request, ct));
        });
    }

    /// <summary>
    /// Client-Roleの一覧を取得する
    /// </summary>
    [HttpGet("FetchClientRoles")]
    public async ValueTask<IActionResult> FetchClientRoles([FromQuery] string clientUuid, CancellationToken ct)
    {
        return await CommonValidationResponse(new FetchClientRolesInput { ClientUuid = clientUuid }, _fetchClientRolesInputValidator, async () =>
        {
            var request = new FetchClientRolesRequest()
            {
                AccessToken = GetAccessTokenByHeader(),
                ClientUuid = clientUuid
            };
            return Ok(await _keycloakService.FetchClientRolesAsync(request, ct));
        });
    }

    /// <summary>
    /// ユーザーに紐づくClient-Roleを取得する
    /// </summary>
    [HttpGet("FetchUserClientRoles")]
    public async ValueTask<IActionResult> FetchUserClientRoles([FromQuery] string userId, [FromQuery] string clientUuid, CancellationToken ct)
    {
        return await CommonValidationResponse(new FetchUserClientRolesInput { UserId = userId, ClientUuid = clientUuid }, _fetchUserClientRolesInputValidator, async () =>
        {
            var request = new FetchUserClientRolesRequest()
            {
                AccessToken = GetAccessTokenByHeader(),
                UserId = userId,
                ClientUuid = clientUuid
            };
            return Ok(await _keycloakService.FetchUserClientRolesAsync(request, ct));
        });
    }

    /// <summary>
    /// ユーザーにClient-Roleをアタッチする
    /// </summary>
    [HttpPost("AddUserClientRoleMappings")]
    public async ValueTask<IActionResult> AddUserClientRoleMappings(AddUserClientRoleMappingInput input, CancellationToken ct)
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
            await _keycloakService.AddUserClientRoleMappingAsync(input.UserId, input.ClientUuid, request, ct);
            return Ok();
        });
    }

    /// <summary>
    /// ユーザーからClient-Roleをデタッチする
    /// </summary>
    [HttpDelete("DeleteUserClientRoleMapping")]
    public async ValueTask<IActionResult> DeleteUserClientRoleMapping(DeleteUserClientRoleMappingInput input, CancellationToken ct)
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
            await _keycloakService.DeleteUserClientRoleMappingAsync(input.UserId, input.ClientUuid, request, ct);
            return Ok();
        });
    }
}
