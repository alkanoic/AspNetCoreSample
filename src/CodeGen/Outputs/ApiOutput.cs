using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TargetNamespace;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TargetController : ControllerBase
{
    private readonly TargetContext _context;

    public TargetController(TargetContext context)
    {
        _context = context;
    }

    // GET: @routePrefix
    [HttpGet]
    public async ValueTask<ActionResult<IEnumerable<TargetModelTypeName>>> Get()
    {
        return await _context.TargetEntitySetName.ToListAsync();
    }

    // GET: @routePrefix/5
    [HttpGet("{id}")]
    public async ValueTask<ActionResult<TargetModelTypeName>> Get(int id)
    {
        var target = await _context.TargetEntitySetName.FindAsync(id);

        if (target == null)
        {
            return NotFound();
        }

        return target;
    }

    // PUT: @routePrefix/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async ValueTask<IActionResult> Put(int id, TargetModelTypeName target)
    {
        if (id != target.id)
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
    public async ValueTask<ActionResult<TargetModelTypeName>> Post(TargetModelTypeName target)
    {
        _context.TargetEntitySetName.Add(target);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (Exists(target.id))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction("Get", new { id = target.id }, target);
    }

    // DELETE: @routePrefix/5
    [HttpDelete("{id}")]
    public async ValueTask<IActionResult> Delete(int id)
    {
        var target = await _context.TargetEntitySetName.FindAsync(id);
        if (target == null)
        {
            return NotFound();
        }

        _context.TargetEntitySetName.Remove(target);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool Exists(int id)
    {
        return _context.TargetEntitySetName.Any(e => e.id == id);
    }
}
