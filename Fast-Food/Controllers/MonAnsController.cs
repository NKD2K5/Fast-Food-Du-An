    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Fast_Food.Models;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Claims;

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
            public async Task<IActionResult> Index(string LoaiSanPham, string TimKiem, int pg = 1)
            {
                const int pageSize = 5; // Số sản phẩm hiển thị trên mỗi trang

                // Truy vấn dữ liệu từ database
                IQueryable<MonAn> monAnQuery = _context.MonAns;

                // Lọc theo Loại Sản Phẩm
                if (!string.IsNullOrEmpty(LoaiSanPham))
                {
                    monAnQuery = monAnQuery.Where(m => m.LoaiSanPham == LoaiSanPham);
                }

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(TimKiem))
                {
                    monAnQuery = monAnQuery.Where(m => m.TenMon.Contains(TimKiem));
                }

                // Tổng số bản ghi sau khi lọc
                int recsCount = await monAnQuery.CountAsync();

                // Tính toán phân trang
                var pager = new demdanhsach(recsCount, pg, pageSize);
                ViewBag.Pager = pager;
                ViewBag.LoaiSanPham = LoaiSanPham;
                ViewBag.TimKiem = TimKiem;

                // Lấy dữ liệu cho trang hiện tại
                var products = await monAnQuery
                    .Skip((pg - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return View(products);
            }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            // Lấy MaKhachHang từ hệ thống phân quyền (Identity)
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (maKhachHang == null)
            {
                return RedirectToAction("Login", "DangNhap");
            }

            int maKH = int.Parse(maKhachHang); // Chuyển đổi về kiểu số nguyên

            // Kiểm tra xem món ăn có tồn tại không
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null)
            {
                return NotFound();
            }

            // Kiểm tra xem món ăn đã có trong giỏ hàng của khách chưa
            var giohang = await _context.GioHangs
                .FirstOrDefaultAsync(m => m.MaMon == id && m.MaKhachHang == maKH);

            if (giohang != null)
            {
                // Nếu đã có thì tăng số lượng lên 1
                giohang.SoLuong += 1;
            }
            else
            {
                // Nếu chưa có thì thêm mới
                giohang = new GioHang
                {
                    MaKhachHang = maKH,
                    MaMon = monAn.MaMon,
                    SoLuong = 1,
                    Gia = monAn.Gia,
                    GhiChu = monAn.TenMon
                };
                _context.GioHangs.Add(giohang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult BuyNow(int id)
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (string.IsNullOrEmpty(maKhachHang))
            {
                return RedirectToAction("Login", "DangNhap"); // Chuyển hướng nếu chưa đăng nhập
            }

            var khachHang = _context.KhachHangs
                .FirstOrDefault(kh => kh.MaKhachHang == int.Parse(maKhachHang));

            if (khachHang == null)
            {
                return BadRequest("Không tìm thấy thông tin khách hàng.");
            }

            var monAn = _context.MonAns.Find(id);
            if (monAn == null || monAn.SoLuong < 1)
            {
                return BadRequest("Sản phẩm không còn hàng.");
            }

            // Tạo hóa đơn mới với thông tin khách hàng
            var hoaDon = new HoaDon
            {
                MaKhachHang = khachHang.MaKhachHang,
                ThoiGianDat = DateTime.Now,
                TrangThaiDonHang = "Chờ xác nhận",
                TrangThaiThanhToan = "Chưa thanh toán",
                SdtlienHe = khachHang.SoDienThoai, // Lấy số điện thoại từ khách hàng
                DiaChiGiaoHang = khachHang.DiaChi, // Lấy địa chỉ từ khách hàng
                DanhGia = 0,
                ChiTietHoaDons = new List<ChiTietHoaDon>()
            };

            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges(); // Lưu để có MaHoaDon

            // Thêm sản phẩm vào chi tiết hóa đơn
            var chiTietHoaDon = new ChiTietHoaDon
            {
                MaHoaDon = hoaDon.MaHoaDon,
                MaMon = id,
                SoLuong = 1,
                Gia = monAn.Gia
            };

            _context.ChiTietHoaDons.Add(chiTietHoaDon);

            // Cập nhật số lượng sản phẩm
            monAn.SoLuong -= 1;
            if (monAn.SoLuong == 0)
            {
                monAn.TrangThai = false; // Cập nhật trạng thái hết hàng
            }

            _context.SaveChanges();

            return RedirectToAction("Details", "HoaDons", new { id = hoaDon.MaHoaDon });
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
            // POST: MonAns/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("MaMon,LoaiSanPham,TenMon,Gia,SoLuong,TrangThai,NgayTao,NgayCapNhat,ChiTietFood")] MonAn monAn, IFormFile HinhAnh)
            {
                // Kiểm tra nếu monAn là null
                if (monAn == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

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

                // Nếu Loại Sản Phẩm là Combo, lấy danh sách các món ăn không phải là Combo để thêm vào ChiTiếtFood
                if (monAn.LoaiSanPham == "7") // "7" là Combo
                {
                    var monAnList = await _context.MonAns.Where(m => m.LoaiSanPham != "Combo").ToListAsync();
                    ViewBag.MonAnList = monAnList;
                }
                else
                {
                    ViewBag.MonAnList = new List<MonAn>(); // Nếu không phải Combo, không cần danh sách ChiTietFood
                }

                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
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

                // Xử lý upload ảnh
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/monan");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var fileExtension = Path.GetExtension(HinhAnh.FileName);
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }

                    monAn.HinhAnh = $"img/monan/{fileName}";
                }

                monAn.NgayTao = DateOnly.FromDateTime(DateTime.Now);
                monAn.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now);

                _context.Add(monAn);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
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
