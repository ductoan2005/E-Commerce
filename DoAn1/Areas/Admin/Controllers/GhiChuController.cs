using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien, user")]
    public class GhiChuController : Controller
    {
        DataContext context;
        public GhiChuController(DataContext context)
        {
            this.context = context;
        }
        // GET: GhiChu
        public ActionResult Index()
        {
            List<GhiChu> th = context.GhiChu.ToList();
            ViewBag.ds = th;
            return View();
        }

        // GET: GhiChu/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GhiChu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GhiChu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GhiChu ghichu)
        {
            GhiChu gc = new GhiChu();
            gc.TenGhiChu = ghichu.TenGhiChu;
            context.GhiChu.Add(gc);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: GhiChu/Edit/5
        public ActionResult Edit(int id)
        {
            GhiChu gc = context.GhiChu.FirstOrDefault(p => p.GhiChuID == id);
            return View(gc);
        }

        // POST: GhiChu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, GhiChu ghichu)
        {
            var gc = context.GhiChu.Find(id);
            gc.TenGhiChu = ghichu.TenGhiChu;
            context.SaveChanges();
            return View();
        }

        // GET: GhiChu/Delete/5
        public ActionResult Delete(int id)
        {
            GhiChu gc = context.GhiChu.FirstOrDefault(p => p.GhiChuID == id);
            return View(gc);
        }

        // POST: GhiChu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, GhiChu ghichu)
        {
            GhiChu gc = context.GhiChu.FirstOrDefault(p => p.GhiChuID == id);
            context.Entry(gc).State = EntityState.Deleted;

            context.SaveChanges();
            return RedirectToAction("Index", "GhiChu");
        }
    }
}