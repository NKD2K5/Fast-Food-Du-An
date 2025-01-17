using Fast_Food.Context;
using Fast_Food.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fast_Food.Controllers
{
    public class DangNhapController : Controller
    {
        readonly DoAnStoreContext _context;

        // Constructor để khởi tạo _context thông qua dependency injection
        public DangNhapController(DoAnStoreContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string tk , string mk)
        {
            var taikhoan = _context.TaiKhoans.FirstOrDefault(t => t.TenTk == tk);
            //check tk có tồn tại không
            if (taikhoan == null)
            {
                ViewBag.loi = "Sai tên đăng nhập!";
                return View();
            }
            //nếu cái tài khoản rồi thì kiểm tra mật khẩu có đúng không
            if (!VerifyPassword(mk, taikhoan.MatKhau))
            {
                ViewBag.loi = "Sai mật khẩu!";
                return View();
            }

            if (taikhoan.LoaiTaiKhoan == "NhanVien")
            {
                // Lưu thông tin nhân viên vào session
                HttpContext.Session.SetString("MaNhanVien", taikhoan.MaNhanVien.ToString());
                HttpContext.Session.SetString("TenNhanVien", taikhoan.TenTk);
            }
            else if (taikhoan.LoaiTaiKhoan == "KhachHang")
            {
                // Lưu thông tin khách hàng vào session
                HttpContext.Session.SetString("MaKhachHang", taikhoan.MaKhachHang.ToString());
                HttpContext.Session.SetString("TenKhachHang", taikhoan.TenTk);
            }

            // Phân quyền và điều hướng theo loại tài khoản
            if (taikhoan.LoaiTaiKhoan == "NhanVien")
            {
                // Chuyển hướng tới giao diện dành cho nhân viên
                return RedirectToAction("Privacy", "Home");
            }
            else if (taikhoan.LoaiTaiKhoan == "KhachHang")
            {
                // Chuyển hướng tới giao diện dành cho khách hàng
                return RedirectToAction("Index", "Home");
            }

            // Nếu không phải loại tài khoản hợp lệ, trả về view mặc định
            return View();
        }
        private bool VerifyPassword(string passwordInput, string passwordStored)
        {
            return passwordInput == passwordStored;
        }
        public IActionResult nhanvien()
        {
            return View();
        }

        public IActionResult khachhang()
        {
            return View();
        }
        public IActionResult DangKy()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
