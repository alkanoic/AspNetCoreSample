using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
__UsingNamespaces__

namespace __NamespaceName__;

/// <summary>
/// This code is automatically generated by CodeGen Api command
/// </summary>
__Authorize__
[Route("api/[controller]")]
[ApiController]
public class __ControllerName__ : ControllerBase
{
    private readonly __ContextTypeName__ _context;

    public __ControllerName__(__ContextTypeName__ context)
    {
        _context = context;
    }

    // GET: @routePrefix
    [HttpGet]
    public async ValueTask<ActionResult<IEnumerable<__ModelTypeName__>>> Get()
    {
        return await _context.__EntitySetName__.ToListAsync();
    }

    // GET: @routePrefix/5
    [HttpGet("__PrimaryKeyNameAttributes__")]
    public async ValueTask<ActionResult<__ModelTypeName__>> Get(__PrimaryKeyArguments__)
    {
        __ContextFindPrimaryKey__
        if (result == null)
        {
            return NotFound();
        }
        return result;
    }

    // PUT: @routePrefix/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("__PrimaryKeyNameAttributes__")]
    public async ValueTask<IActionResult> Put(__PrimaryKeyArguments__, __ModelTypeName__ target)
    {
        if (__CompareTargetToArguments__)
        {
            return BadRequest();
        }

        _context.Entry(target).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(__PrimaryKeyNameArguments__))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: @routePrefix
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async ValueTask<ActionResult<__ModelTypeName__>> Post(__ModelTypeName__ target)
    {
        _context.__EntitySetName__.Add(target);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (Exists(__PrimaryKeyNameTargetArguments__))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction("Get", new { __PrimaryKeyNameNewObject__ }, target);
    }

    // DELETE: @routePrefix/5
    [HttpDelete("__PrimaryKeyNameAttributes__")]
    public async ValueTask<IActionResult> Delete(__PrimaryKeyArguments__)
    {
        __ContextFindPrimaryKey__
        if (result == null)
        {
            return NotFound();
        }

        _context.__EntitySetName__.Remove(result);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool Exists(__PrimaryKeyArguments__)
    {
        __EntitySetExist__
    }
}
