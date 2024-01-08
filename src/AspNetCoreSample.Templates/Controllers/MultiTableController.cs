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
    public class MultiTableController : Controller
    {
        private readonly SampleContext _context;

        public MultiTableController(SampleContext context)
        {
            _context = context;
        }

        // GET: MultiTable
        public async Task<IActionResult> Index()
        {
            return View(await _context.MultiTables.ToListAsync());
        }

        // GET: MultiTable/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multiTable = await _context.MultiTables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multiTable == null)
            {
                return NotFound();
            }

            return View(multiTable);
        }

        // GET: MultiTable/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MultiTable/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Charid,TargetName,TargetInt,TargetDecimal,TargetDate,TargetBit,CreateAt,CreateUser,UpdateAt,UpdateUser")] MultiTable multiTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(multiTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(multiTable);
        }

        // GET: MultiTable/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multiTable = await _context.MultiTables.FindAsync(id);
            if (multiTable == null)
            {
                return NotFound();
            }
            return View(multiTable);
        }

        // POST: MultiTable/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Charid,TargetName,TargetInt,TargetDecimal,TargetDate,TargetBit,CreateAt,CreateUser,UpdateAt,UpdateUser")] MultiTable multiTable)
        {
            if (id != multiTable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(multiTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MultiTableExists(multiTable.Id))
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
            return View(multiTable);
        }

        // GET: MultiTable/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multiTable = await _context.MultiTables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (multiTable == null)
            {
                return NotFound();
            }

            return View(multiTable);
        }

        // POST: MultiTable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var multiTable = await _context.MultiTables.FindAsync(id);
            if (multiTable != null)
            {
                _context.MultiTables.Remove(multiTable);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MultiTableExists(int id)
        {
            return _context.MultiTables.Any(e => e.Id == id);
        }
    }
}
