using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DoAn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        UserManager<KhachHang> userManager;
        DataContext context;
        IMapper mapper;
        public CustomerController(UserManager<KhachHang> userManager, DataContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.context = context;
            this.mapper = mapper;
        }
        // GET: Customer
        public ActionResult Index()
        {
            var user = userManager.Users;
            ViewBag.user = user;
            return View();
        }

        // GET: Customer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Customer/Create
        public ActionResult CreateUser()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(AdminKhachHangDangKiModel khachhang)
        {
            if (!ModelState.IsValid)
            {
                return View(khachhang);
            }
            else
            {
                var user = mapper.Map<KhachHang>(khachhang);
                user.UserName = khachhang.Email;
                user.HinhAnh = "user.png";
                user.TenKH = khachhang.FirstName + khachhang.LastName;
                user.DienThoaiKH = khachhang.DienThoaiKH;
                var result = await userManager.CreateAsync(user, khachhang.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View(khachhang);
                }
                else
                {

                    await userManager.AddToRoleAsync(user, "Customer");
                }
                context.KhachHang.AddRange(user);

                return RedirectToAction(nameof(CustomerController.Index), "Customer");
            }
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(string id)
        {
            KhachHang oldKhachHang = context.KhachHang.FirstOrDefault(p => p.Id == id);
            return View(oldKhachHang);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, KhachHang khachhang, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                KhachHang oldKhachHang = context.KhachHang.FirstOrDefault(p => p.Id == id);
                oldKhachHang.TenKH = khachhang.TenKH;
                oldKhachHang.DiaChiKH = khachhang.DiaChiKH;
                oldKhachHang.DienThoaiKH = khachhang.DienThoaiKH;
                oldKhachHang.TichDiem = khachhang.TichDiem;
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
        public async Task<ActionResult> ChangePassword(string id, KhachHang khachhang)
        {
            if (ModelState.IsValid)
            {
                KhachHang oldKhachHang = context.KhachHang.FirstOrDefault(p => p.Id == id);
                oldKhachHang.Password = khachhang.Password;

                oldKhachHang.PasswordHash = userManager.PasswordHasher.HashPassword(oldKhachHang,khachhang.Password);
                var result = await userManager.UpdateAsync(oldKhachHang);
                if(result.Succeeded)
                {
                    ViewBag.Status = 1;
                }
            }
            context.SaveChanges();
            return View(khachhang);
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
        public IActionResult CancelVoucher(int mavoucher, string id)
        {
            var voucher = context.Voucher.Find(mavoucher);
            voucher.TrangThai = -1;
            context.Entry(voucher).State = EntityState.Modified;
            context.SaveChanges();
            return Redirect("~/Admin/Customer/Voucher/"+ id);
        }
        public IActionResult GenVoucher()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GenVoucher(string id, int discount)
        {
            var kh = context.KhachHang.FirstOrDefault(x => x.Id == id);
            
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