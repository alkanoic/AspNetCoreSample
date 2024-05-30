using AspNetCoreSample.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSample.Mvc.Controllers;

public class HtmxApiController : ControllerBase
{
    private readonly ILogger<HtmxApiController> _logger;

    public HtmxApiController(ILogger<HtmxApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public FetchClass Fetch(string request)
    {
        return new FetchClass() { Value1 = request, Value2 = "abc" };
    }

    [HttpPost]
    public FetchClass FetchPost([FromBody] RequestClass request)
    {
        return new FetchClass() { Value1 = request.Request ?? "", Value2 = "abc" };
    }
}

public class RequestClass
{
    public string? Request { get; set; }
}

public class FetchClass
{
    public required string Value1 { get; set; }

    public required string Value2 { get; set; }
}
