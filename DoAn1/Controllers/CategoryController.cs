using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAn1.Models;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext context;
        public CategoryController(DataContext context)
        {
            this.context = context;
        }
        public IActionResult List()
        {
            context.ThuongHieu.ToList();
            return View();
        }
        public IActionResult Index(int id)
        {
            List<Laptop> details = context.Laptop.Include(l => l.ThuongHieu).Where(l=>l.ThuongHieu.ThuongHieuID==id).ToList();
            if (details.Count.Equals(0)) return NotFound();
            ViewBag.laptop = details;
            return View();
        }
    }
}