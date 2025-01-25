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
        public async Task<IActionResult> Index(string LoaiSanPham)
        {
            IQueryable<MonAn> monAnQuery = _context.MonAns;

            if (!string.IsNullOrEmpty(LoaiSanPham))
            {
                // Lọc theo loại món ăn hoặc combo
                monAnQuery = monAnQuery.Where(m => m.LoaiSanPham == LoaiSanPham);
            }

            var monAns = await monAnQuery.ToListAsync();

            return View(monAns);
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
            // Kiểm tra nếu LoaiSanPham là "0" (Lựa Chọn Món Ăn)
            if (monAn.LoaiSanPham == "0")
            {
                ModelState.AddModelError("LoaiSanPham", "Vui lòng chọn loại món ăn hợp lệ.");
                ViewBag.LoaiSanPhamList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "---Lựa Chọn Món Ăn---" },
                    new SelectListItem { Value = "1", Text = "Gà" },
                    new SelectListItem { Value = "2", Text = "Kem" },
                    new SelectListItem { Value = "3", Text = "Burger" },
                    new SelectListItem { Value = "4", Text = "Pizza" },
                    new SelectListItem { Value = "5", Text = "Khoai" },
                    new SelectListItem { Value = "6", Text = "Đồ Uống" },
                    new SelectListItem { Value = "7", Text = "Combo" }
                };
                return View(monAn);
            }

            // Nếu tất cả các điều kiện hợp lệ, tiếp tục thêm sản phẩm
            if (ModelState.IsValid)
            {
                _context.Add(monAn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi validation, giữ lại danh sách và trả về View
            ViewBag.LoaiSanPhamList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "---Lựa Chọn Món Ăn---" },
                new SelectListItem { Value = "1", Text = "Gà" },
                new SelectListItem { Value = "2", Text = "Kem" },
                new SelectListItem { Value = "3", Text = "Burger" },
                new SelectListItem { Value = "4", Text = "Pizza" },
                new SelectListItem { Value = "5", Text = "Khoai" },
                new SelectListItem { Value = "6", Text = "Đồ Uống" },
                new SelectListItem { Value = "7", Text = "Combo" }
            };

            return View(monAn);
        }


        public IActionResult CapNhatAnh()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatAnh(IFormFile hinhAnhFile, int maMon)
        {
            if (hinhAnhFile != null && hinhAnhFile.Length > 0)
            {
                // Lấy đường dẫn thư mục để lưu ảnh
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", hinhAnhFile.FileName);
                Console.WriteLine($"File Path: {filePath}");  // Log để kiểm tra đường dẫn

                // Lưu file ảnh vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hinhAnhFile.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn ảnh trong cơ sở dữ liệu
                var monAn = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMon == maMon);
                if (monAn != null)
                {
                    monAn.HinhAnh = "/images/" + hinhAnhFile.FileName;  // Cập nhật đường dẫn ảnh
                    _context.Update(monAn);
                    await _context.SaveChangesAsync();
                }

                // Quay lại trang chi tiết món ăn (hoặc trang nào đó)
                return RedirectToAction("Details", new { id = maMon });
            }
            else
            {
                // Nếu không có ảnh được tải lên, bạn có thể thêm thông báo lỗi
                ModelState.AddModelError("", "Vui lòng chọn một ảnh để tải lên.");
                return View();
            }
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
