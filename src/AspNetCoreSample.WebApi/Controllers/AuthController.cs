using AspNetCoreSample.WebApi.Services.Token;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly ITokenService _tokenService;

    public AuthController(ILogger<AuthController> logger, ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    [HttpPost]
    public async ValueTask<TokenResponse> Token(TokenRequest request)
    {
        return await _tokenService.GetTokenAsync(request);
    }

    [HttpPost("UpdateToken")]
    public async ValueTask<TokenResponse> UpdateToken(UpdateTokenRequest request)
    {
        return await _tokenService.UpdateTokenAsync(request);
    }

    [Authorize]
    [HttpGet("Sample")]
    public ValueTask<string> Sample(string sample)
    {
        return ValueTask.FromResult(sample);
    }

    [Authorize(Policy = "User")]
    [HttpGet("SampleUser")]
    public ValueTask<string> SampleUser(string sample)
    {
        return ValueTask.FromResult(sample);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("SampleAdmin")]
    public ValueTask<string> SampleAdmin(string sample)
    {
        return ValueTask.FromResult(sample);
    }
}
