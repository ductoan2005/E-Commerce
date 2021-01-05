using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using DoAn1.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CartController : Controller
    {
        DataContext context;
        IMapper mapper;
        private readonly UserManager<KhachHang> _userManager;
        public CartController(DataContext context, IMapper mapper, UserManager<KhachHang> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var hd = context.HoaDon.ToList();
            List<HoaDon> listhd = new List<HoaDon>();
            foreach(var item in hd)
            {
                GhiChu gc = context.GhiChu.Find(item.GhiChuID);
                KhachHang kh = context.KhachHang.Find(item.KhachHangID);
                HoaDon a = context.HoaDon.Find(item.HoaDonID);
                a.KhachHang = kh;
                a.GhiChu = gc;
                listhd.Add(a);
            }    

            //ViewBag.KH = context.KhachHang.Find(hd.KhachHangID);

            return View(listhd);
        }
        public IActionResult List(DateTime fromdate, DateTime tillday)
        {
            var hd = context.HoaDon.Where(x=>x.Ngay >= fromdate && x.Ngay <= tillday).ToList();
            List<HoaDon> listhd = new List<HoaDon>();
            foreach (var item in hd)
            {
                GhiChu gc = context.GhiChu.Find(item.GhiChuID);
                KhachHang kh = context.KhachHang.Find(item.KhachHangID);
                HoaDon a = context.HoaDon.Find(item.HoaDonID);
                a.KhachHang = kh;
                a.GhiChu = gc;
                listhd.Add(a);
            }
            return View(listhd);
        }
        public IActionResult CancelList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelList(DateTime fromdate, DateTime tillday)
        {
            var hd = context.HoaDon.Where(x => x.Ngay >= fromdate && x.Ngay <= tillday && x.TrangThai == 0).ToList();
            List<HoaDon> listhd = new List<HoaDon>();
            foreach (var item in hd)
            {
                GhiChu gc = context.GhiChu.Find(item.GhiChuID);
                KhachHang kh = context.KhachHang.Find(item.KhachHangID);
                HoaDon a = context.HoaDon.Find(item.HoaDonID);
                a.KhachHang = kh;
                a.GhiChu = gc;
                listhd.Add(a);
            }
            return View(listhd);
        }
        public IActionResult SuccessList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuccessList(DateTime fromdate, DateTime tillday)
        {
            var hd = context.HoaDon.Where(x => x.Ngay >= fromdate && x.Ngay <= tillday && x.TrangThai == 4).ToList();
            List<HoaDon> listhd = new List<HoaDon>();
            foreach (var item in hd)
            {
                GhiChu gc = context.GhiChu.Find(item.GhiChuID);
                KhachHang kh = context.KhachHang.Find(item.KhachHangID);
                HoaDon a = context.HoaDon.Find(item.HoaDonID);
                a.KhachHang = kh;
                a.GhiChu = gc;
                listhd.Add(a);
            }
            return View(listhd);
        }
        public IActionResult ReturnList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnList(DateTime fromdate, DateTime tillday)
        {
            var hd = context.HoaDon.Where(x => x.Ngay >= fromdate && x.Ngay <= tillday && x.TrangThai == -1).ToList();
            List<HoaDon> listhd = new List<HoaDon>();
            foreach (var item in hd)
            {
                GhiChu gc = context.GhiChu.Find(item.GhiChuID);
                KhachHang kh = context.KhachHang.Find(item.KhachHangID);
                HoaDon a = context.HoaDon.Find(item.HoaDonID);
                a.KhachHang = kh;
                a.GhiChu = gc;
                listhd.Add(a);
            }
            return View(listhd);
        }
        public async Task<IActionResult> OutputExcelAsync(int id)
        {

            HoaDon  hd = context.HoaDon.Find(id);
            Voucher voucher = context.Voucher.Find(hd.VoucherID);
            KhachHang kh = await _userManager.FindByIdAsync(hd.KhachHangID);
            List<CT_HoaDon> ctHD = context.CT_HoaDon.Where(s => s.HoaDonID == hd.HoaDonID).ToList();
            double thanhtien = 0;
            string contentType = "application/vnd.openxlmformats-officedocument.spreadsheetml.sheet";
            string fileName = "HoaDon.xlsx";
            Table tb = new Table();
            
            try
            {
                var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Hóa đơn");

                //Order code
                var rgcode = worksheet.Range("B1:H1").Merge();
                rgcode.Value = hd.HoaDonID;
                worksheet.Cell(1, 1).Value = "Mã hóa đơn:";



                //Header
                var rngheader = worksheet.Range("A2:H2");
                rngheader.Style.Font.FontSize = 26;
                rngheader.Merge().Value = "Hóa đơn mua hàng:";
                rngheader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Date
                worksheet.Cell(3, 2).Value = "Ngày:";
                var rgdate = worksheet.Range("C3:G3");
                rgdate.Style.DateFormat.Format = "dd/MM/yyyy";
                rgdate.Merge().Value = hd.Ngay;
                rgdate.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                worksheet.Range("A4:H4").Merge();
                worksheet.Range("A9:H9").Merge();
                //Name customer
                worksheet.Cell(5, 1).Value = "Khách hàng:";
                var rgcus = worksheet.Range("B5:H5");
                rgcus.Merge().Value = kh.TenKH;
                rgcus.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Address
                worksheet.Cell(6, 1).Value = "Địa chỉ:";
                var rgadd = worksheet.Range("B6:H6");
                rgadd.Merge().Value = kh.DiaChiKH;
                rgadd.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Phone number
                worksheet.Cell(7, 1).Value = "Số điện thoại:";
                var rgphone = worksheet.Range("B7:H7");
                rgphone.Merge().Value = kh.DienThoaiKH;
                rgphone.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Email
                worksheet.Cell(8, 1).Value = "Email:";
                var rgemail = worksheet.Range("B8:H8");
                rgemail.Merge().Value = kh.Email;
                rgemail.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                //Title
                var rgtitle = worksheet.Range("A10:H10");
                worksheet.Cell(10, 1).Value = "STT";
                worksheet.Cell(10, 2).Value = "Tên mặt hàng";
                worksheet.Cell(10, 3).Value = "Thương hiệu";
                worksheet.Cell(10, 4).Value = "Số lượng";
                worksheet.Cell(10, 5).Value = "Giá tiền";
                worksheet.Cell(10, 6).Value = "Mã giảm giá";
                worksheet.Cell(10, 7).Value = "Thành tiền";
                worksheet.Cell(10, 8).Value = "Đã thanh toán";

                for (int index = 1; index <= ctHD.Count; index++)
                {
                    Laptop lt = context.Laptop.Find(ctHD[index - 1].LaptopID);
                    ThuongHieu th = context.ThuongHieu.Find(lt.ThuongHieuID);
                        
                    worksheet.Cell(index + 10, 1).Value = index;
                    worksheet.Cell(index + 10, 2).Value = lt.TenLaptop;
                    worksheet.Cell(index + 10, 3).Value = th.TenThuongHieu;
                    worksheet.Cell(index + 10, 4).Value = ctHD[index - 1].SoLuong;
                    worksheet.Cell(index + 10, 5).Value = "$" + lt.Giatien;
                    worksheet.Cell(index + 10, 6).Value = voucher.Discount + "%";
                    worksheet.Cell(index + 10, 7).Value = "$" + ctHD[index - 1].TongTien;
                    thanhtien = thanhtien + ctHD[index - 1].TongTien;
                    if(hd.TrangThai == 5)
                    {
                        worksheet.Cell(index + 10, 8).Value = "$" + Math.Round(thanhtien * ((100 - voucher.Discount) * 0.01),2) + " (Thanh Toán Paypal, $" + Math.Round((thanhtien * (0.01 * voucher.Discount)),2)  + "Mã giảm giá)";
                        thanhtien = 0;
                    }
                    else
                    {
                        worksheet.Cell(index + 10, 8).Value = "$" + Math.Round(Convert.ToDecimal(thanhtien * (voucher.Discount * 0.01)),2) + " (" + voucher.MaVoucher + ")";
                        thanhtien = thanhtien * ((100 - voucher.Discount) * 0.01);
                        
                    }      
                }
                
                

                
                worksheet.Cell(ctHD.Count + 12, 5).Value = "Tổng cộng phải trả";
                worksheet.Cell(ctHD.Count + 12, 6).Value = "$" + Math.Round(Convert.ToDecimal(thanhtien),2);
                


                int t = ctHD.Count + 12;
                int x = ctHD.Count + 11;
                string b = "H" + x;
                worksheet.Range("A10:" + b).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range("A10:" + b).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                string a = "H" + t;
                worksheet.Range("A1:" + a).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
            catch 
            {
                return Redirect("Index");
            }
        }
        
        public IActionResult ShipperOrder(int id)
        {
            HoaDon hd = context.HoaDon.Find(id);
            hd.TrangThai = 2;
            context.Entry(hd).State = EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult SubmitOrder(int id)
        {
            HoaDon hd = context.HoaDon.Find(id);
            hd.TrangThai = 4;
            context.Entry(hd).State = EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult ShippingOrder(int id)
        {
            HoaDon hd = context.HoaDon.Find(id);
            hd.TrangThai = 3;
            context.Entry(hd).State = EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction("Index"); 
        }
        public IActionResult CancelOrder(int id)
        {
            HoaDon hd = context.HoaDon.Find(id);
            hd.TrangThai = 0;
            context.Entry(hd).State = EntityState.Modified;
            context.SaveChanges();

            var cthd = context.CT_HoaDon.Where(s => s.HoaDonID == hd.HoaDonID).ToList();
            foreach (var item in cthd)
            {
                Laptop lt = context.Laptop.Find(item.LaptopID);
                lt.SoLuong += item.SoLuong;
                context.Entry(lt).State = EntityState.Modified;
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult ReturnOrder(int id)
        {
            ViewBag.ghichu = context.GhiChu.ToList();
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnOrder(int id, int ghichu)
        {
            HoaDon hd = context.HoaDon.Find(id);
            hd.TrangThai = -1;
            hd.GhiChuID = ghichu;
            context.Entry(hd).State = EntityState.Modified;
            context.SaveChanges();

            var cthd = context.CT_HoaDon.Where(s => s.HoaDonID == hd.HoaDonID).ToList();
            foreach (var item in cthd)
            {
                Laptop lt = context.Laptop.Find(item.LaptopID);
                lt.SoLuong += item.SoLuong;
                context.Entry(lt).State = EntityState.Modified;
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
