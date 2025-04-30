using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using AspNetCoreSample.WebApi;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSample.WebApi.Validators;
using AspNetCoreSample.DataModel.Models;
using AspNetCoreSample.Util.Logging;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbAccessController : ControllerBase
{
    private readonly SampleContext _sampleContext;
    public DbAccessController(SampleContext sampleContext)
    {
        _sampleContext = sampleContext;
    }

    [Logging]
    public async ValueTask<IEnumerable<Name>> Get()
    {
        return await _sampleContext.Names.ToListAsync();
    }

    [Logging]
    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] Name name)
    {
        var validator = new NameValidator();
        var validationResult = validator.Validate(name);

        if (!validationResult.IsValid)
        {
            return BadRequest(CommonValidation.GetValidationErrors(validationResult));
        }

        try
        {
            _sampleContext.Add(name);
            _sampleContext.Names.Add(name);
            await _sampleContext.SaveChangesAsync();
            return Ok(100);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
