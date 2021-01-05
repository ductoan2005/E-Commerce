using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DoAn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien, user")]
    public class ChartController : Controller
    {
        DataContext context;
        public ChartController(DataContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]

        public JsonResult Chart()
        {
            var cthd = context.CT_HoaDon.ToList();
            return Json(cthd);
        }
        [HttpGet]
        public JsonResult ProductChart()
        {
            var laptops = context.Laptop.ToList();
            return Json(laptops);
        }
        public IActionResult ProductChartIndex()
        {
            return View();
        }
        public IActionResult DoanhThu(DateTime FromDate, DateTime ToDate, int option)
        {
            double sum = 0;
            if (option == 0)
            {
                sum = context.HoaDon.Where(a => a.Ngay >= FromDate && a.Ngay <= ToDate).Sum(c => c.TongTien);

            }
            else
            {
                sum = context.HoaDon.Where(a => a.Ngay >= FromDate && a.Ngay <= ToDate && a.TrangThai == option).Sum(c => c.TongTien);

            }
            ViewBag.sum = sum;
            ViewBag.FromDate = FromDate.ToString("dd MMM yyyy");
            ViewBag.ToDate = ToDate.ToString("dd MMM yyyy");
            return View("index");
        }
    }
}