using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace @NamespaceName@;

@Authorize@
[Route("api/[controller]")]
[ApiController]
public class @ControllerName@ : ControllerBase
{
    private readonly @ContextTypeName@ _context;

    public @ControllerName@(@ContextTypeName@ context)
    {
        _context = context;
    }

// GET: @routePrefix
[HttpGet]
public async ValueTask<ActionResult<IEnumerable<@ModelTypeName@>>> Get()
    {
        return await _context.@EntitySetName@.ToListAsync();
    }

// GET: @routePrefix/5
[HttpGet("{id}")]
public async ValueTask<ActionResult<@ModelTypeName@>> Get(@PrimaryKeyShortTypeName@ id)
{
    var target = await _context.@EntitySetName@.FindAsync(id);

    if (target == null)
    {
        return NotFound();
    }

    return target;
}

// PUT: @routePrefix/5
// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
[HttpPut("{id}")]
public async ValueTask<IActionResult> Put(@PrimaryKeyShortTypeName@ id, @ModelTypeName@ target)
{
    if (id != target.@PrimaryKeyName@)
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
        if (!Exists(id))
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
public async ValueTask<ActionResult<@ModelTypeName@>> Post(@ModelTypeName@ target)
{
    _context.@EntitySetName@.Add(target);

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
        if (Exists(target.@PrimaryKeyName@))
        {
            return Conflict();
        }
        else
        {
            throw;
        }
    }

    return CreatedAtAction("Get", new { id = target.@PrimaryKeyName@ }, target);
}

// DELETE: @routePrefix/5
[HttpDelete("{id}")]
public async ValueTask<IActionResult> Delete(@PrimaryKeyShortTypeName@ id)
{
    var target = await _context.@EntitySetName@.FindAsync(id);
    if (target == null)
    {
        return NotFound();
    }

    _context.@EntitySetName@.Remove(target);
    await _context.SaveChangesAsync();

    return NoContent();
}

private bool Exists(@PrimaryKeyShortTypeName@ id)
{
    return _context.@EntitySetName@.Any(e => e.@PrimaryKeyName@ == id);
}
}
