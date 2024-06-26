using AspNetCoreSample.WebApi.Models;
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

    /// <summary>
    /// Keycloakによる認証を行う
    /// </summary>
    /// <param name="request">ユーザ名およびパスワード</param>
    /// <returns>AccessTokenおよびRefreshTokenの取得</returns>
    [HttpPost("Auth")]
    public async ValueTask<IActionResult> Auth(TokenRequest request)
    {
        try
        {
            var result = await _tokenRequestValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }
            return Ok(await _tokenService.AuthTokenAsync(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// RefreshTokenを使用してAccessTokenを更新する
    /// </summary>
    /// <param name="request">RefreshToken</param>
    /// <returns>AccessTokenおよびRefreshTokenの取得</returns>
    [HttpPost("RefreshToken")]
    public async ValueTask<IActionResult> RefreshToken(UpdateTokenRequest request)
    {
        try
        {
            var result = await _updateTokenRequestValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }
            return Ok(await _tokenService.RefreshTokenAsync(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }

    /// <summary>
    /// RefreshTokenを無効化する
    /// </summary>
    /// <param name="request">RefreshToken</param>
    /// <returns>なし</returns>
    [HttpPost("RevokeToken")]
    public async ValueTask<IActionResult> RevokeToken(RevokeTokenRequest request)
    {
        try
        {
            var result = await _revokeTokenRequestValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var errors = new WebApiFailResponse(result);
                return BadRequest(errors);
            }
            await _tokenService.RevokeTokenAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new WebApiFailResponse(ex));
        }
    }
}
