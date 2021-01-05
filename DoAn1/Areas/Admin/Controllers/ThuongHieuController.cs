using DoAn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien, user")]
    public class ThuongHieuController : Controller
    {
        DataContext context;
        public ThuongHieuController(DataContext context)
        {
            this.context = context;
        }
        // GET: ThuongHieuController
        public ActionResult Index()
        {
            List<ThuongHieu> th = context.ThuongHieu.ToList();
            ViewBag.ds = th;
            return View();
        }

        // GET: ThuongHieuController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ThuongHieuController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ThuongHieuController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThuongHieu thuonghieu)
        {
            ThuongHieu th = new ThuongHieu();
            th.TenThuongHieu = thuonghieu.TenThuongHieu;
            context.ThuongHieu.Add(th);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ThuongHieuController/Edit/5
        public ActionResult Edit(int id)
        {
            ThuongHieu th = context.ThuongHieu.FirstOrDefault(p => p.ThuongHieuID == id);
            return View(th);
        }

        // POST: ThuongHieuController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ThuongHieu thuonghieu)
        {
            var th = context.ThuongHieu.Find(id);
            th.TenThuongHieu = thuonghieu.TenThuongHieu;
            context.SaveChanges();
            return View();
        }

        // GET: ThuongHieuController/Delete/5
        public ActionResult Delete(int id)
        {

            ThuongHieu th = context.ThuongHieu.FirstOrDefault(p => p.ThuongHieuID == id);
            return View(th);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            ThuongHieu th = context.ThuongHieu.FirstOrDefault(p => p.ThuongHieuID == id);
            context.Entry(th).State = EntityState.Deleted;

            context.SaveChanges();
            return RedirectToAction("Index", "ThuongHieu");
        }
    }
}
