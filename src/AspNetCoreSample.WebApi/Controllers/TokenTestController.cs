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

    [Authorize]
    public ValueTask<SampleJsonRequest> SampleJson(SampleJsonRequest request)
    {
        return ValueTask.FromResult(request);
    }

    public class SampleJsonRequest
    {
        public required string Value1 { get; set; }
        public required string Value2 { get; set; }
    }
}
