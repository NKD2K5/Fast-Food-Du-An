using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fast_Food.Models;

namespace Fast_Food.Controllers
{
    public class ChiTietComboesController : Controller
    {
        private readonly DoAnStoreContext _context;

        public ChiTietComboesController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: ChiTietComboes
        public async Task<IActionResult> Index()
        {
            var doAnStoreContext = _context.ChiTietCombos.Include(c => c.MaMonNavigation);
            return View(await doAnStoreContext.ToListAsync());
        }

        // GET: ChiTietComboes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietCombo = await _context.ChiTietCombos
                .Include(c => c.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaCombo == id);
            if (chiTietCombo == null)
            {
                return NotFound();
            }

            return View(chiTietCombo);
        }

        // GET: ChiTietComboes/Create
        public IActionResult Create()
        {
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon");
            return View();
        }

        // POST: ChiTietComboes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCombo,MaMon")] ChiTietCombo chiTietCombo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chiTietCombo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietCombo.MaMon);
            return View(chiTietCombo);
        }

        // GET: ChiTietComboes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietCombo = await _context.ChiTietCombos.FindAsync(id);
            if (chiTietCombo == null)
            {
                return NotFound();
            }
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietCombo.MaMon);
            return View(chiTietCombo);
        }

        // POST: ChiTietComboes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCombo,MaMon")] ChiTietCombo chiTietCombo)
        {
            if (id != chiTietCombo.MaCombo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietCombo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietComboExists(chiTietCombo.MaCombo))
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
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", chiTietCombo.MaMon);
            return View(chiTietCombo);
        }

        // GET: ChiTietComboes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietCombo = await _context.ChiTietCombos
                .Include(c => c.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaCombo == id);
            if (chiTietCombo == null)
            {
                return NotFound();
            }

            return View(chiTietCombo);
        }

        // POST: ChiTietComboes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiTietCombo = await _context.ChiTietCombos.FindAsync(id);
            if (chiTietCombo != null)
            {
                _context.ChiTietCombos.Remove(chiTietCombo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietComboExists(int id)
        {
            return _context.ChiTietCombos.Any(e => e.MaCombo == id);
        }
    }
}
