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
    private readonly IValidator<CreateUserInput> _createUserInputValidator;
    private readonly IValidator<UpdateUserInput> _updateUserInputValidator;
    private readonly IValidator<ChangePasswordInput> _changePasswordInputValidator;
    private readonly IValidator<ResetPasswordByEmailInput> _resetPasswordByEmailInputValidator;
    private readonly IValidator<DeleteUserInput> _deleteUserInputValidator;

    public KeycloakController(ILogger<KeycloakController> logger,
        IKeycloakService keycloakService,
        IValidator<CreateUserInput> createUserInputValidator,
        IValidator<UpdateUserInput> updateUserInputValidator,
        IValidator<ChangePasswordInput> changePasswordInputValidator,
        IValidator<ResetPasswordByEmailInput> resetPasswordByEmailInputValidator,
        IValidator<DeleteUserInput> deleteUserInputValidator)
    {
        _logger = logger;
        _keyclaokService = keycloakService;
        _createUserInputValidator = createUserInputValidator;
        _updateUserInputValidator = updateUserInputValidator;
        _changePasswordInputValidator = changePasswordInputValidator;
        _resetPasswordByEmailInputValidator = resetPasswordByEmailInputValidator;
        _deleteUserInputValidator = deleteUserInputValidator;
    }

    /// <summary>
    /// ユーザーを作成する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    [HttpPost("CreateUser")]
    public async ValueTask<IActionResult> CreateUser(CreateUserInput input)
    {
        try
        {
            var result = await _createUserInputValidator.ValidateAsync(input);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }

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
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    [HttpPut("UpdateUser")]
    public async ValueTask<IActionResult> UpdateUser(UpdateUserInput input)
    {
        try
        {
            var result = await _updateUserInputValidator.ValidateAsync(input);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }

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
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// ユーザーのパスワードを変更する
    /// </summary>
    /// <param name="input">パスワード情報</param>
    [HttpPut("ChangePassword")]
    public async ValueTask<IActionResult> ChangePassword(ChangePasswordInput input)
    {
        try
        {
            var result = await _changePasswordInputValidator.ValidateAsync(input);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }

            var request = new ChangePasswordRequest()
            {
                UserId = input.UserId,
                Credential = new Credential(input.Password)
            };
            await _keyclaokService.ChangePasswordAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// ユーザーのパスワードをリセットするためのメールを送信
    /// </summary>
    [HttpPut("ResetPasswordByEmail")]
    public async ValueTask<IActionResult> ResetPasswordByEmail(ResetPasswordByEmailInput input)
    {
        try
        {
            var result = await _resetPasswordByEmailInputValidator.ValidateAsync(input);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }

            var request = new ResetPasswordByEmailRequest()
            {
                UserId = input.UserId,
            };
            await _keyclaokService.ResetPasswordByEmailAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// ユーザーを削除する
    /// </summary>
    /// <param name="input">ユーザー情報</param>
    [HttpDelete("DeleteUser")]
    public async ValueTask<IActionResult> DeleteUser(DeleteUserInput input)
    {
        try
        {
            var result = await _deleteUserInputValidator.ValidateAsync(input);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }

            var request = new DeleteUserRequest()
            {
                UserId = input.UserId,
            };
            await _keyclaokService.DeleteUserAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }
}
