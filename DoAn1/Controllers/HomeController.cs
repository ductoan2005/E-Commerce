using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DoAn1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace DoAn1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            this.context = context;
            _logger = logger;
        }

        public IActionResult Index(int? id)
        {
            int k = 0;
            if (id != null)
            {
                k = id.GetValueOrDefault() * 5;
            }
            List<Laptop> lt = context.Laptop.OrderBy(s => s.LaptopID).Skip(k).Take(5).Include(l => l.ThuongHieu).ToList();
            ViewBag.laptop = lt;
            ViewBag.count = context.Laptop.Count() / 5;
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laptop = await context.Laptop.Include(l => l.ThuongHieu)
                .FirstOrDefaultAsync(m => m.LaptopID == id);

            var dsdanhgia = context.Comment.Where(a => a.LaptopID == id).Include(l=>l.KhachHang).ToList();
            var SLComment = context.Comment.Where(a => a.LaptopID == id).Count();
            ViewBag.dsdanhgia = dsdanhgia;
            ViewBag.SLComment = SLComment;
            if (context.Comment.Where(a => a.LaptopID == id).Count() > 0)
            {
                double sum = context.Comment.Where(a => a.LaptopID == id).Average(a => a.Rating);
                ViewBag.sumrating = sum;
            }
            if (laptop == null)
            {
                return NotFound();
            }
            return View(laptop);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateCmt(int laptopID, string text, int rating)
        {
            var comment = new Comment();
            comment.Text = text;
            comment.KhachHangID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            comment.CmtDate = DateTime.Now;
            comment.LaptopID = laptopID;
            comment.Rating = rating;
            context.Comment.Add(comment);
            context.SaveChanges();
            // return View("index","homepage");
            return RedirectToAction("Details", new RouteValueDictionary(new { Controller = "Home", Action = "Details", id = laptopID }));
        }
        public IActionResult Find(string tensp)
        {
            List<Laptop> laptop = context.Laptop.Where(s => s.TenLaptop.Contains(tensp)).ToList();
            ViewBag.laptop = laptop;
            return View();
        }
        public IActionResult SortByName()
        {
            List<Laptop> laptop = context.Laptop.OrderBy(s => s.TenLaptop).ToList();
            ViewBag.laptop = laptop;
            return View();
        }
        public IActionResult SortByPrice()
        {
            List<Laptop> laptop = context.Laptop.OrderBy(s => s.Giatien).ToList();
            ViewBag.laptop = laptop;
            return View();
        }
        public IActionResult SetTheme(string data)
        {
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Append("theme", data, cookieOptions);
            return RedirectToAction("Index");
        }
        public IActionResult SearchOrder()
        {
            return View();
        }
        
        public IActionResult ResultSearchOrder(string email)
        {
            var hd = context.HoaDon.Where(s => s.KhachHangID == email).ToList();
            ViewBag.dsOrder = context.HoaDon.Where(s => s.KhachHangID == email).ToList();
            return View(hd);
        }
        public IActionResult DetailsMyOder(int id)
        {
            var ds = context.CT_HoaDon.Where(s => s.HoaDonID == id).ToList();
            List<CT_HoaDon> ct = new List<CT_HoaDon>();
            foreach (var item in ds)
            {
                Laptop lt = context.Laptop.Find(item.LaptopID);
                CT_HoaDon a = context.CT_HoaDon.Find(item.CT_HoaDonID);
                a.Laptop = lt;
                ct.Add(a);
            }
            return View(ct);
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
            hd.GhiChuID = ghichu;
            hd.TrangThai = 0;
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
