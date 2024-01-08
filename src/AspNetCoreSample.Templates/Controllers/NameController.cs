using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreSample.Templates.Models;

// auto generated code
namespace AspNetCoreSample.Templates.Controllers
{
    public class NameController : Controller
    {
        private readonly SampleContext _context;

        public NameController(SampleContext context)
        {
            _context = context;
        }

        // GET: Name
        public async Task<IActionResult> Index()
        {
            return View(await _context.Names.ToListAsync());
        }

        // GET: Name/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var name = await _context.Names
                .FirstOrDefaultAsync(m => m.Id == id);
            if (name == null)
            {
                return NotFound();
            }

            return View(name);
        }

        // GET: Name/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Name/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name1")] Name name)
        {
            if (ModelState.IsValid)
            {
                _context.Add(name);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(name);
        }

        // GET: Name/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var name = await _context.Names.FindAsync(id);
            if (name == null)
            {
                return NotFound();
            }
            return View(name);
        }

        // POST: Name/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name1")] Name name)
        {
            if (id != name.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(name);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NameExists(name.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(name);
        }

        // GET: Name/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var name = await _context.Names
                .FirstOrDefaultAsync(m => m.Id == id);
            if (name == null)
            {
                return NotFound();
            }

            return View(name);
        }

        // POST: Name/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var name = await _context.Names.FindAsync(id);
            if (name != null)
            {
                _context.Names.Remove(name);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NameExists(int id)
        {
            return _context.Names.Any(e => e.Id == id);
        }
    }
}
