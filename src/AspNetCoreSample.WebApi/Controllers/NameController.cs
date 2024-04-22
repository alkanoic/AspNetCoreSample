using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AspNetCoreSample.WebApi.Models;

using Google.Protobuf.WellKnownTypes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSample.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NameController : ControllerBase
{
    private readonly SampleContext _context;

    public NameController(SampleContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async ValueTask<IEnumerable<Name>> Names()
    {
        return await _context.Names.ToListAsync();
    }

    [HttpPost]
    public async ValueTask Create(Name name)
    {
        _context.Names.Add(name);
        await _context.SaveChangesAsync();
    }

    [HttpDelete]
    public async ValueTask Delete(int id)
    {
        if (_context.Names == null)
        {
            return;
        }
        var name = await _context.Names.FindAsync(id);
        if (name != null)
        {
            _context.Names.Remove(name);
        }

        await _context.SaveChangesAsync();
    }
}
