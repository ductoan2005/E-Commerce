using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoAn1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Controllers
{
    public class CustomerController : Controller
    {
        private readonly DataContext context;
        private readonly UserManager<KhachHang> userManager;
        public CustomerController(DataContext context, UserManager<KhachHang> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var hd = context.HoaDon.Where(x => x.KhachHangID == id).ToList();
            int tichdiem = 0;
            foreach(var item in hd)
            {
                if(item.TrangThai == 4)
                {
                    var cthd = context.CT_HoaDon.Where(x => x.HoaDonID == item.HoaDonID).ToList();
                    foreach (var ct_hd in cthd)
                    {
                        tichdiem += (ct_hd.TongTien * 10 / 100);
                    }
                }
            }
            var laptop = context.KhachHang.FirstOrDefault(m => m.Id == id);
            laptop.TichDiem = tichdiem;
            context.SaveChanges();
            if (laptop == null)
            {
                return NotFound();
            }

            return View(laptop);
        }
        public IActionResult Edit(string id)
        {
            KhachHang oldKhachHang = context.KhachHang.FirstOrDefault(p => p.Id == id);
            return View(oldKhachHang);
        }
        [HttpPost]
        public IActionResult Edit(string id, KhachHang khachhang, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                KhachHang oldKhachHang = context.KhachHang.FirstOrDefault(p => p.Id == id);
                oldKhachHang.TenKH = khachhang.TenKH;
                oldKhachHang.DiaChiKH = khachhang.DiaChiKH;
                oldKhachHang.DienThoaiKH = khachhang.DienThoaiKH;
               
                if (photo == null)
                {
                    oldKhachHang.HinhAnh = "user.png";
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/userimage", photo.FileName);
                    var stream = new FileStream(path, FileMode.Create);

                    photo.CopyToAsync(stream);
                    oldKhachHang.HinhAnh = photo.FileName;
                }
                   
                ViewBag.Status = 1;
            }
            context.SaveChanges();
            return View(khachhang);
        }
        public IActionResult ChangePassword(string id)
        {
            KhachHang oldKhachHang = context.KhachHang.FirstOrDefault(p => p.Id == id);
            return View(oldKhachHang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string id, string newpassword, string oldpassword)
        {
            if (ModelState.IsValid)
            {
                KhachHang oldKhachHang = context.KhachHang.FirstOrDefault(p => p.Id == id);
                if (oldKhachHang.Password != oldpassword)
                {
                    ViewBag.Message = "Mật khẩu cũ không chính xác!";
                    return View();
                }
                if (newpassword == oldpassword)
                {
                    ViewBag.Message = "Mật khẩu mới không được trùng với mật khẩu cũ!";
                    return View();
                }
                if (oldpassword == null)
                {
                    ViewBag.Message = "Chưa nhập mật khẩu cũ";
                    return View();
                }
                if (newpassword == null)
                {
                    ViewBag.Message = "Chưa nhập mật khẩu mới";
                    return View();
                } 
                oldKhachHang.Password = newpassword;

                oldKhachHang.PasswordHash = userManager.PasswordHasher.HashPassword(oldKhachHang, newpassword);
                var result = await userManager.UpdateAsync(oldKhachHang);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Đổi mật khẩu thành công";
                }
            }
            context.SaveChanges();
            return View();
        }
        public IActionResult MyOrder(string id)
        {
            KhachHang kh = context.KhachHang.Find(id);
            var hd = context.HoaDon.Where(s => s.KhachHangID == kh.Id).ToList();
            ViewBag.dsOrder = context.HoaDon.Where(s => s.KhachHangID == kh.Id).ToList();
            return View(hd);
        }
        public IActionResult CancelOrder(int id)
        {
            ViewBag.gc = context.GhiChu.ToList();
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public IActionResult CancelOrder(int id, int ghichu)
        {
            var hd = context.HoaDon.Find(id);
            var cthd = context.CT_HoaDon.Where(x => x.HoaDonID == hd.HoaDonID).ToList();
            foreach (var item in cthd)
            {
                var shoes = context.Laptop.FirstOrDefault(x => x.LaptopID == item.LaptopID);
                shoes.SoLuong += item.SoLuong;
                context.SaveChanges();
            }
            hd.GhiChuID = ghichu;
            hd.TrangThai = 0;
            context.SaveChanges();
            return RedirectToAction("Index","Home");
        }
        public IActionResult DetailsMyOder(int id)
        {
            var ds = context.CT_HoaDon.Where(s => s.HoaDonID == id).ToList();
            List<CT_HoaDon> ct = new List<CT_HoaDon>();
            foreach(var item in ds)
            {
                Laptop lt = context.Laptop.Find(item.LaptopID);
                CT_HoaDon a = context.CT_HoaDon.Find(item.CT_HoaDonID);
                a.Laptop = lt;
                ct.Add(a);
            }
            return View(ct);
        }
        public IActionResult Voucher(string id)
        {
            var voucher = context.Voucher.Where(x => x.KhachHangID == id).ToList();
            foreach (var item in voucher)
            {
                if (item.NgayHetHan == DateTime.Now)
                {
                    item.TrangThai = 2;
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            ViewBag.voucher = voucher;
            return View();
        }
        public IActionResult GenVoucher()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GenVoucher(string id)
        {
            int discount = 0;
            var kh = context.KhachHang.FirstOrDefault(x => x.Id == id);          
            if (kh.TichDiem == 0)
            {
                ViewBag.MaVoucher = "Bạn Không Đủ Điều Kiện Để Tạo Mã";
            }
            else
            {
                if (kh.TichDiem >= 50 && kh.TichDiem <= 99) discount = 10;
                if (kh.TichDiem >= 100 && kh.TichDiem <= 199) discount = 20;
                if (kh.TichDiem >= 200) discount = 30;
                Voucher vc = new Voucher()
                {
                    MaVoucher = RandomString(6, false),
                    KhachHangID = id,
                    Discount = discount,
                    TrangThai = 1,
                    NgayTao = DateTime.Now,
                    NgayHetHan = DateTime.Now.AddDays(1),
                };
                context.Voucher.Add(vc);
                context.SaveChanges();
                ViewBag.MaVoucher = vc.MaVoucher;
            }
            return View();
        }
        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}