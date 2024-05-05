using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimpleController : ControllerBase
{
    private readonly ILogger<SimpleController> _logger;

    public SimpleController(ILogger<SimpleController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "Simple")]
    public SimpleOutput Get([FromQuery] SimpleInput input)
    {
        return new SimpleOutput { Output = input.Input };
    }

    [HttpPost]
    public SimpleOutput SavePost(SimpleInput input)
    {
        return new SimpleOutput { Output = input.Input };
    }
}
