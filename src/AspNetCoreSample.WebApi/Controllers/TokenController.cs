using AspNetCoreSample.WebApi.Services.Keycloak.Token;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IValidator<TokenRequest> _tokenRequestValidator;
    private readonly IValidator<UpdateTokenRequest> _updateTokenRequestValidator;
    private readonly IValidator<RevokeTokenRequest> _revokeTokenRequestValidator;

    public TokenController(ILogger<TokenController> logger,
        ITokenService tokenService,
        IValidator<TokenRequest> tokenRequestValidator,
        IValidator<UpdateTokenRequest> updateTokenRequestValidator,
        IValidator<RevokeTokenRequest> revokeTokenRequestValidator)
    {
        _logger = logger;
        _tokenService = tokenService;
        _tokenRequestValidator = tokenRequestValidator;
        _updateTokenRequestValidator = updateTokenRequestValidator;
        _revokeTokenRequestValidator = revokeTokenRequestValidator;
    }

    [HttpPost]
    public async ValueTask<IActionResult> Token(TokenRequest request)
    {
        try
        {
            var result = await _tokenRequestValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            return Ok(await _tokenService.AuthTokenAsync(request));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("exception", ex.Message);
            return BadRequest(ModelState);
        }
    }

    [HttpPost("UpdateToken")]
    public async ValueTask<IActionResult> UpdateToken(UpdateTokenRequest request)
    {
        try
        {
            var result = await _updateTokenRequestValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            return Ok(await _tokenService.UpdateTokenAsync(request));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("exception", ex.Message);
            return BadRequest(ModelState);
        }
    }

    [HttpPost("RevokeToken")]
    public async ValueTask<IActionResult> RevokeToken(RevokeTokenRequest request)
    {
        try
        {
            var result = await _revokeTokenRequestValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            await _tokenService.RevokeTokenAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("exception", ex.Message);
            return BadRequest(ModelState);
        }
    }
}
