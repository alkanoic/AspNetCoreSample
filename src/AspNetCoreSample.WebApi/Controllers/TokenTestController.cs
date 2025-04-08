using AspNetCoreSample.WebApi.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenTestController : ControllerBase
{
    private readonly ILogger<TokenTestController> _logger;

    public TokenTestController(ILogger<TokenTestController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    [Logging]
    [HttpGet("Sample")]
    public ValueTask<string> Sample(string sample)
    {
        return ValueTask.FromResult(sample);
    }

    [Authorize(Policy = "User")]
    [Logging]
    [HttpGet("SampleUser")]
    public ValueTask<string> SampleUser(string sample)
    {
        return ValueTask.FromResult(sample);
    }

    [Authorize(Policy = "Admin")]
    [Logging]
    [HttpGet("SampleAdmin")]
    public ValueTask<string> SampleAdmin(string sample)
    {
        return ValueTask.FromResult(sample);
    }
}
