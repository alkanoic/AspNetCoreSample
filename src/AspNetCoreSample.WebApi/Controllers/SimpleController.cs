// using AspNetCoreSample.Util;
using AspNetCoreSample.WebApi.Resources;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimpleController : ControllerBase
{
    private readonly ILogger<SimpleController> _logger;
    private readonly ISharedResource _commonResource;
    private readonly IStringLocalizer<SimpleController> _localizer;
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

    public SimpleController(ILogger<SimpleController> logger,
                            ISharedResource commonResource,
                            IStringLocalizer<SharedResource> sharedResource,
                            IStringLocalizer<SimpleController> localizer)
    {
        _logger = logger;
        _commonResource = commonResource;
        _sharedLocalizer = sharedResource;
        _localizer = localizer;
    }

    [HttpGet(Name = "Simple")]
    public SimpleOutput Get([FromQuery] SimpleInput input)
    {
        return new SimpleOutput { Output = input.Input };
    }

    [Logging]
    [HttpPost]
    public SimpleOutput SavePost(SimpleInput input)
    {
        return new SimpleOutput { Output = input.Input };
    }

    [HttpGet("Resource")]
    public IActionResult Resource()
    {
        return Ok(new
        {
            ControllerResource = _localizer["UserIdLength"].Value,
            SharedResource2 = _sharedLocalizer["UserIdLength"].Value,
            SharedResource = _commonResource.RoleIdLength
        });
    }
}
