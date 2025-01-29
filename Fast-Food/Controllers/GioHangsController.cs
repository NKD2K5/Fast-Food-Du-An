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
    public class GioHangsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public GioHangsController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: GioHangs
        public async Task<IActionResult> Index()
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (maKhachHang == null)
            {
                return RedirectToAction("Login", "DangNhap");
            }

            int maKH = int.Parse(maKhachHang);

            var gioHangData = await _context.GioHangs
                .Include(g => g.MaKhachHangNavigation) // Bao gồm thông tin khách hàng
                .Include(g => g.MaMonNavigation) // Bao gồm thông tin món ăn
                .Where(g => g.MaKhachHang == maKH)
                .ToListAsync();

            return View(gioHangData);
        }


        // GET: GioHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.MaComboNavigation)
                .Include(g => g.MaKhachHangNavigation)
                .Include(g => g.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaGioHang == id);
            if (gioHang == null)
            {
                return NotFound();
            }

            return View(gioHang);
        }

        // GET: GioHangs/Create
        public IActionResult Create()
        {
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon");
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang");
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon");
            return View();
        }

        // POST: GioHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGioHang,MaKhachHang,MaMon,MaCombo,TenSanPham,Gia,SoLuong,GhiChu")] GioHang gioHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gioHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaCombo);
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", gioHang.MaKhachHang);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaMon);
            return View(gioHang);
        }

        // GET: GioHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHang = await _context.GioHangs.FindAsync(id);
            if (gioHang == null)
            {
                return NotFound();
            }
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaCombo);
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", gioHang.MaKhachHang);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaMon);
            return View(gioHang);
        }

        // POST: GioHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaGioHang,MaKhachHang,MaMon,MaCombo,TenSanPham,Gia,SoLuong,GhiChu")] GioHang gioHang)
        {
            if (id != gioHang.MaGioHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gioHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GioHangExists(gioHang.MaGioHang))
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
            ViewData["MaCombo"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaCombo);
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", gioHang.MaKhachHang);
            ViewData["MaMon"] = new SelectList(_context.MonAns, "MaMon", "MaMon", gioHang.MaMon);
            return View(gioHang);
        }

        // GET: GioHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.MaComboNavigation)
                .Include(g => g.MaKhachHangNavigation)
                .Include(g => g.MaMonNavigation)
                .FirstOrDefaultAsync(m => m.MaGioHang == id);
            if (gioHang == null)
            {
                return NotFound();
            }

            return View(gioHang);
        }

        // POST: GioHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gioHang = await _context.GioHangs.FindAsync(id);
            if (gioHang != null)
            {
                _context.GioHangs.Remove(gioHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GioHangExists(int id)
        {
            return _context.GioHangs.Any(e => e.MaGioHang == id);
        }
    }
}
