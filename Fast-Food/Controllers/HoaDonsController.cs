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
    public class HoaDonsController : Controller
    {
        private readonly DoAnStoreContext _context;

        public HoaDonsController(DoAnStoreContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        public async Task<IActionResult> Index()
        {
            var doAnStoreContext = _context.HoaDons.Include(h => h.MaKhachHangNavigation).Include(h => h.MaNhanVienNavigation).Include(h => h.MaVoucherNavigation);
            return View(await doAnStoreContext.ToListAsync());
        }

        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.MaNhanVienNavigation)
                .Include(h => h.MaVoucherNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: HoaDons/Create
        public IActionResult Create()
        {
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang");
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien");
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher");
            return View();
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHoaDon,MaKhachHang,MaNhanVien,ThoiGianDat,ThoiGianKetThuc,TrangThaiDonHang,TrangThaiThanhToan,SdtlienHe,DiaChiGiaoHang,DanhGia,MaVoucher")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", hoaDon.MaKhachHang);
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", hoaDon.MaNhanVien);
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher", hoaDon.MaVoucher);
            return View(hoaDon);
        }

        // GET: HoaDons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", hoaDon.MaKhachHang);
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", hoaDon.MaNhanVien);
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher", hoaDon.MaVoucher);
            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHoaDon,MaKhachHang,MaNhanVien,ThoiGianDat,ThoiGianKetThuc,TrangThaiDonHang,TrangThaiThanhToan,SdtlienHe,DiaChiGiaoHang,DanhGia,MaVoucher")] HoaDon hoaDon)
        {
            if (id != hoaDon.MaHoaDon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHoaDon))
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
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", hoaDon.MaKhachHang);
            ViewData["MaNhanVien"] = new SelectList(_context.NhanViens, "MaNhanVien", "MaNhanVien", hoaDon.MaNhanVien);
            ViewData["MaVoucher"] = new SelectList(_context.Vouchers, "MaVoucher", "MaVoucher", hoaDon.MaVoucher);
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaKhachHangNavigation)
                .Include(h => h.MaNhanVienNavigation)
                .Include(h => h.MaVoucherNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.MaHoaDon == id);
        }
    }
}
