using AspNetCoreSample.WebApi.Models;
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

    public KeycloakController(ILogger<KeycloakController> logger,
        IKeycloakService keycloakService,
        IValidator<CreateUserInput> createUserInputValidator)
    {
        _logger = logger;
        _keyclaokService = keycloakService;
        _createUserInputValidator = createUserInputValidator;
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
                Credentials = new List<CreateUserRequest.Credential>{
                    new() {
                        Type = "password",
                        Value = input.Password,
                        Temporary = false
                    }
                }
            };
            await _keyclaokService.CreateUserAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("exception", ex.Message);
            return BadRequest(ModelState);
        }
    }

}
