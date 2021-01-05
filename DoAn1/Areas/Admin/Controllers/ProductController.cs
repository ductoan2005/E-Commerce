using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DoAn1.Models;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien, user")]
    public class ProductController : Controller
    {
        DataContext context;
        IMapper mapper;
        public ProductController(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public IActionResult Index(int? id)
        {
            int k = 0;
            if (id != null)
            {
                k = id.GetValueOrDefault() * 5;
            }
            List<Laptop> lt = context.Laptop.OrderBy(s => s.LaptopID).Skip(k).Take(5).Include(l => l.ThuongHieu).ToList();
            ViewBag.ds = lt;
            ViewBag.count = context.Laptop.Count() / 5;
            return View();
        }
        [HttpGet]
        public IActionResult Add()
        {
            Laptop product = new Laptop();
            ViewBag.StateList = context.ThuongHieu.ToList();
            return View(product);
        }
        [HttpPost]
        public IActionResult Add(Laptop laptop, IFormFile photo)
        {

            if (ModelState.IsValid)
            {
                Laptop newLaptop = new Laptop();
                if (photo == null)
                {
                    newLaptop.HinhAnh = "iphone-11-pro-max-green-600x600.jpg";
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/image", photo.FileName);
                    var stream = new FileStream(path, FileMode.Create);

                    photo.CopyToAsync(stream);
                    newLaptop.HinhAnh = photo.FileName;
                }
                newLaptop.TenLaptop = laptop.TenLaptop;
                newLaptop.ThongSo = laptop.ThongSo;
                newLaptop.SoLuong = laptop.SoLuong;
                newLaptop.Giatien = laptop.Giatien;
                newLaptop.NgayTao = DateTime.Now;
                newLaptop.GioiThieu = laptop.GioiThieu;
                newLaptop.ThuongHieuID = laptop.ThuongHieuID;

                context.Laptop.Add(newLaptop);
                context.SaveChanges();

                return RedirectToAction("Index", "Product");
            }
            else
            {
                return View(laptop);
            }
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Laptop oldLaptop = context.Laptop.FirstOrDefault(p => p.LaptopID == id);
            return View(oldLaptop);
        }
        [HttpPost]
        public IActionResult Edit(int id, Laptop laptop)
        {
            if (ModelState.IsValid)
            {
                Laptop oldLaptop = context.Laptop.FirstOrDefault(p => p.LaptopID == id);
                oldLaptop.TenLaptop = laptop.TenLaptop;
                oldLaptop.SoLuong = laptop.SoLuong;
                oldLaptop.ThongSo = laptop.ThongSo;
                oldLaptop.Giatien = laptop.Giatien;
                oldLaptop.NgayTao = DateTime.Now;
                ViewBag.Status = 1;
            }
            context.SaveChanges();
            return View(laptop);
        }
        public IActionResult Delete(int id)
        {

            Laptop oldLaptop = context.Laptop.FirstOrDefault(p => p.LaptopID == id);
            return View(oldLaptop);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            Laptop laptop = context.Laptop.FirstOrDefault(p => p.LaptopID == id);
            context.Entry(laptop).State = EntityState.Deleted;

            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

       
    }
}