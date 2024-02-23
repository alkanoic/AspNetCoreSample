using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AspNetCoreSample.Templates.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.Templates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameApiController : ControllerBase
    {
        private readonly SampleContext _context;

        public NameApiController(SampleContext context)
        {
            _context = context;
        }

        // GET: api/NameApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Name>>> GetNames()
        {
            return await _context.Names.ToListAsync();
        }

        // GET: api/NameApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Name>> GetName(int id)
        {
            var name = await _context.Names.FindAsync(id);

            if (name == null)
            {
                return NotFound();
            }

            return name;
        }

        // PUT: api/NameApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutName(int id, Name name)
        {
            if (id != name.Id)
            {
                return BadRequest();
            }

            _context.Entry(name).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NameExists(id))
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

        // POST: api/NameApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Name>> PostName(Name name)
        {
            _context.Names.Add(name);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetName", new { id = name.Id }, name);
        }

        // DELETE: api/NameApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteName(int id)
        {
            var name = await _context.Names.FindAsync(id);
            if (name == null)
            {
                return NotFound();
            }

            _context.Names.Remove(name);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NameExists(int id)
        {
            return _context.Names.Any(e => e.Id == id);
        }
    }
}
