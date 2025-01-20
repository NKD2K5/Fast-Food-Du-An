using Fast_Food.Models;
using Microsoft.EntityFrameworkCore;

namespace Fast_Food
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Đăng ký DbContext với chuỗi kết nối
            builder.Services.AddDbContext<DoAnStoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            // Đăng ký Session
            builder.Services.AddSession(options => options.IOTimeout = TimeSpan.FromSeconds(5));

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Cấu hình pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=DangNhap}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
 