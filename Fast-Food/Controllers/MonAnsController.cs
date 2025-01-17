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
    public class MonAnsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public MonAnsController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: MonAns
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonAns.ToListAsync());
        }

        // GET: MonAns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monAn = await _context.MonAns
                .FirstOrDefaultAsync(m => m.MaMon == id);
            if (monAn == null)
            {
                return NotFound();
            }

            return View(monAn);
        }

        // GET: MonAns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MonAns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMon,LoaiSanPham,TenMon,Gia,SoLuong,TrangThai,NgayTao,NgayCapNhat,ChiTietFood,HinhAnh")] MonAn monAn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monAn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monAn);
        }

        // GET: MonAns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null)
            {
                return NotFound();
            }
            return View(monAn);
        }

        // POST: MonAns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaMon,LoaiSanPham,TenMon,Gia,SoLuong,TrangThai,NgayTao,NgayCapNhat,ChiTietFood,HinhAnh")] MonAn monAn)
        {
            if (id != monAn.MaMon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monAn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonAnExists(monAn.MaMon))
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
            return View(monAn);
        }

        // GET: MonAns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monAn = await _context.MonAns
                .FirstOrDefaultAsync(m => m.MaMon == id);
            if (monAn == null)
            {
                return NotFound();
            }

            return View(monAn);
        }

        // POST: MonAns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn != null)
            {
                _context.MonAns.Remove(monAn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonAnExists(int id)
        {
            return _context.MonAns.Any(e => e.MaMon == id);
        }
    }
}
