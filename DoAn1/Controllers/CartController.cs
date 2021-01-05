using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ClosedXML.Excel;
using DoAn1.EmailSender;
using DoAn1.Helper;
using DoAn1.Models;
using DoAn1.PaypalHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DoAn1.Controllers
{
    public class CartController : Controller
    {
        public IConfiguration configuration { get; }

        IEmailSender emailsender;
        UserManager<KhachHang> userManager;
        DataContext context;
        IMapper mapper;
        public CartController(IConfiguration configuration, DataContext context, IMapper mapper, UserManager<KhachHang> userManager, IEmailSender emailsender)
        {
            this.userManager = userManager;
            this.context = context;
            this.mapper = mapper;
            this.emailsender = emailsender;
            this.configuration = configuration;
        }
        private void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
        private int isExist(int id)
        {
            List<GioHang> cart = context.GioHang.ToList();
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].LaptopID == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public IActionResult NotFound()
        {
            return View();
        }
        public IActionResult Index(string id)
        {
            List<GioHang> cart = context.GioHang.Where(x => x.KhachHangID == id).ToList();
            if (cart.Count() == 0) return View("NotFound");
            foreach(var item in cart)
            {
                var lt = context.Laptop.FirstOrDefault(x => x.LaptopID == item.LaptopID);
                
            }
            ViewBag.cart = cart;
            return View();
        }
        [HttpPost]
        
        public IActionResult Buy(int id, int soLuong, string khachhangid)
        {
            
            if ((context.GioHang.Where(x=>x.KhachHangID == khachhangid)).Count() == 0) 
            {
                GioHang giohang = new GioHang();
                giohang.LaptopID = id;
                giohang.Laptop = context.Laptop.Find(id);
                giohang.SoLuong = soLuong;
                giohang.KhachHangID = khachhangid;
                context.GioHang.Add(giohang);
                context.SaveChanges();
            }
            else
            {
                List<GioHang> cart = context.GioHang.Where(x => x.KhachHangID == khachhangid).ToList();
                int index = isExist(id);
                Laptop lt = context.Laptop.Find(id);
                if (index != -1)
                {
                    if (cart[index].SoLuong + soLuong > lt.SoLuong)
                    {
                        SetAlert("Sản phẩm trong kho chỉ còn " + lt.SoLuong + ". ^-^", "error");
                        return RedirectToAction("Index");
                    }
                    cart[index].SoLuong += soLuong;
                }
                else
                {
                    GioHang giohang = new GioHang();
                    giohang.LaptopID = id;
                    giohang.Laptop = context.Laptop.Find(id);
                    giohang.SoLuong = soLuong;
                    giohang.KhachHangID = khachhangid;
                    context.GioHang.Add(giohang);
                }
                context.SaveChanges();
            }
            return RedirectToAction("Index","Home");
        }
        
        public IActionResult CheckOut(string voucher, string id)
        {
            double total = 0;
            int thanhtien = 0;
            List<GioHang> cart = context.GioHang.Where(x => x.KhachHangID == id).ToList();
            foreach (var item in cart)
            {
                var lt = context.Laptop.FirstOrDefault(x => x.LaptopID == item.LaptopID);
                thanhtien = lt.Giatien * item.SoLuong;
                total += thanhtien;
            }
            var vc = context.Voucher.FirstOrDefault(x => x.MaVoucher == voucher);
            if (vc == null) ViewBag.Message = "Voucher Không Tồn Tại";

            else if (voucher == null)
            {
                ViewBag.Message = "Không có voucher";
            }

            else if (vc.TrangThai == 1)
            {
                total = total * ((100 - vc.Discount) * 0.01);
                ViewBag.Message = "Sử dụng voucher thành công";
            }
            else if (vc.TrangThai == 0) ViewBag.Message = "Voucher Đã Sử Dụng";
            else if (vc.TrangThai == 2) ViewBag.Message = "Voucher Đã Hết Hạn Sử Dụng";
            else if (vc.TrangThai == -1) ViewBag.Message = "Voucher Đã Bị Huỷ Bởi Admin";
                       
            ViewBag.cart = cart;
            ViewBag.voucher = voucher;
            ViewBag.total = Math.Round(Convert.ToDecimal(total), 2); ;
            return View();
        }
        public IActionResult CheckOutDone(string id, string voucher)
        {
            
            var kh = context.KhachHang.Find(id);
            if ((kh.DiaChiKH == null) || (kh.DienThoaiKH == null) || (kh.TenKH == null)) return Redirect("~/Customer/Edit/" + id);
            int ma = 0;
            int total = 0;
            List<GioHang> cart = context.GioHang.Where(x => x.KhachHangID == id).ToList();
            
            var vc = context.Voucher.FirstOrDefault(x => x.MaVoucher == voucher);
            if (voucher == null)
            {
                ma = 0;
                
                HoaDon hoaDon = new HoaDon()
                {
                    KhachHangID = userManager.GetUserId(User),
                    Ngay = DateTime.Now,
                    TrangThai = 1,
                    
                };
                
                context.HoaDon.Add(hoaDon);
                context.SaveChanges();
                foreach (var item in cart)
                {
                    var ltop = context.Laptop.FirstOrDefault(x => x.LaptopID == item.LaptopID);
                    CT_HoaDon ct_HoaDon = new CT_HoaDon()
                    {
                        Ngay = DateTime.Now,
                        HoaDonID = hoaDon.HoaDonID,
                        LaptopID = item.LaptopID,
                        SoLuong = item.SoLuong,
                        TongTien = ltop.Giatien * item.SoLuong,
                        

                    };
                    total += ltop.Giatien * item.SoLuong;
                    Laptop lt = context.Laptop.Find(ct_HoaDon.LaptopID);
                    lt.SoLuong = lt.SoLuong - item.SoLuong;
                    context.Entry(lt).State = EntityState.Modified;
                    context.CT_HoaDon.Add(ct_HoaDon);
                    context.SaveChanges();
                }
                hoaDon.TongTien = total;
                context.Entry(hoaDon).State = EntityState.Modified;
                context.SaveChanges();
            }
            else if(voucher == vc.MaVoucher && vc.TrangThai == 1)
            {
                ma = context.Voucher.FirstOrDefault(x => x.MaVoucher == voucher).VoucherID;
                
                HoaDon hoaDon = new HoaDon()
                {
                    KhachHangID = userManager.GetUserId(User),
                    Ngay = DateTime.Now,
                    TrangThai = 1,
                    VoucherID = ma,
                };
                context.HoaDon.Add(hoaDon);
                context.SaveChanges();
                foreach (var item in cart)
                {
                    var ltop = context.Laptop.FirstOrDefault(x => x.LaptopID == item.LaptopID);
                    CT_HoaDon ct_HoaDon = new CT_HoaDon()
                    {
                        Ngay = DateTime.Now,
                        HoaDonID = hoaDon.HoaDonID,
                        LaptopID = item.LaptopID,
                        SoLuong = item.SoLuong,
                        TongTien = ltop.Giatien * item.SoLuong,

                    };
                    total += ltop.Giatien * item.SoLuong;
                    Laptop lt = context.Laptop.Find(ct_HoaDon.LaptopID);
                    lt.SoLuong = lt.SoLuong - item.SoLuong;
                    context.Entry(lt).State = EntityState.Modified;
                    context.CT_HoaDon.Add(ct_HoaDon);
                    context.SaveChanges();
                }
                vc.TrangThai = 0;
                context.SaveChanges();
            }
            else if(vc.TrangThai == 2)
            {
                return View("CheckOut");
            }
            SendEmail(userManager.GetUserId(User));
            foreach (var item in cart)
            {
                context.Entry(item).State = EntityState.Deleted;
                context.SaveChanges();
            }
            ViewBag.cart = cart;
            ViewBag.total = total;
            ViewBag.voucher = voucher;
            return View();
        }
        [Route("remove/{id}")]
        [HttpGet]
        public IActionResult Remove(int id)
        {
            GioHang cart = context.GioHang.Find(id);
            context.Entry(cart).State = EntityState.Deleted;
            context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Success()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SendEmail(string id)
        {
            var emailcustomer = context.KhachHang.Find(id).Email;
            var message = new Message(new string[] { emailcustomer },
                "Thanks for buying!", 
                "Cảm ơn quý khách đã mua hàng của chúng tôi! đơn hàng sẽ được vận chuyển nhanh nhất cho quý khách!");
            emailsender.SendEmail(message);

            return RedirectToAction("CheckOutDone");
        }
        [HttpPost]
        public async Task<IActionResult> Checkoutpaypal(double total, string id, string voucher)
        {
            var kh = context.KhachHang.Find(id);
            if ((kh.DiaChiKH == null) || (kh.DienThoaiKH == null) || (kh.TenKH == null)) return Redirect("~/Customer/Edit/" + id);
            int ma = 0;
            
            if (voucher == null) ma = 0;
            else ma = context.Voucher.FirstOrDefault(x => x.MaVoucher == voucher).VoucherID;
            var vc = context.Voucher.FirstOrDefault(x => x.MaVoucher == voucher);
            List<GioHang> cart = context.GioHang.Where(x => x.KhachHangID == id).ToList();

            if (voucher == null)
            {
                ma = 0;

                HoaDon hoaDon = new HoaDon()
                {
                    KhachHangID = userManager.GetUserId(User),
                    Ngay = DateTime.Now,
                    TrangThai = 5,
                    VoucherID = null,
                };

                context.HoaDon.Add(hoaDon);
                context.SaveChanges();
                foreach (var item in cart)
                {
                    var ltop = context.Laptop.FirstOrDefault(x => x.LaptopID == item.LaptopID);
                    CT_HoaDon ct_HoaDon = new CT_HoaDon()
                    {
                        Ngay = DateTime.Now,
                        HoaDonID = hoaDon.HoaDonID,
                        LaptopID = item.LaptopID,
                        SoLuong = item.SoLuong,
                        TongTien = ltop.Giatien * item.SoLuong,

                    };
                    total += ltop.Giatien * item.SoLuong;
                    Laptop lt = context.Laptop.Find(ct_HoaDon.LaptopID);
                    lt.SoLuong = lt.SoLuong - item.SoLuong;
                    context.Entry(lt).State = EntityState.Modified;
                    context.CT_HoaDon.Add(ct_HoaDon);
                    context.SaveChanges();
                }
            }
            else if (voucher == vc.MaVoucher && vc.TrangThai == 1)
            {
                ma = context.Voucher.FirstOrDefault(x => x.MaVoucher == voucher).VoucherID;

                HoaDon hoaDon = new HoaDon()
                {
                    KhachHangID = userManager.GetUserId(User),
                    Ngay = DateTime.Now,
                    TrangThai = 5,
                    VoucherID = ma,
                };
                context.HoaDon.Add(hoaDon);
                context.SaveChanges();
                foreach (var item in cart)
                {
                    var ltop = context.Laptop.FirstOrDefault(x => x.LaptopID == item.LaptopID);
                    CT_HoaDon ct_HoaDon = new CT_HoaDon()
                    {
                        Ngay = DateTime.Now,
                        HoaDonID = hoaDon.HoaDonID,
                        LaptopID = item.LaptopID,
                        SoLuong = item.SoLuong,
                        TongTien = ltop.Giatien * item.SoLuong,

                    };
                    total += ltop.Giatien * item.SoLuong;
                    Laptop lt = context.Laptop.Find(ct_HoaDon.LaptopID);
                    lt.SoLuong = lt.SoLuong - item.SoLuong;
                    context.Entry(lt).State = EntityState.Modified;
                    context.CT_HoaDon.Add(ct_HoaDon);
                    context.SaveChanges();
                }
                vc.TrangThai = 0;
                context.SaveChanges();
            }
            else if (vc.TrangThai == 2)
            {
                return View("CheckOut");
            }
            vc.TrangThai = 0;
            context.SaveChanges();
            foreach(var item in cart)
            {
                context.Entry(item).State = EntityState.Deleted;
                context.SaveChanges();
            }           
            var payPalAPI = new PayPalAPI(configuration);
            string url = await payPalAPI.getRedirectURLToPayPal(total);         
            return Redirect(url);
        }
        [Route("success")]
        public async Task<IActionResult> Success([FromQuery(Name ="paymentId")] string paymentId,[FromQuery(Name ="PayerID")] string payerID)
        {
            var payPalAPI = new PayPalAPI(configuration);
            PayPalPaymentExecutedResponse result = await payPalAPI.executedPayment(paymentId,payerID);
            Debug.WriteLine("Transaction Detail");
            Debug.WriteLine("cart:" + result.cart);
            Debug.WriteLine("create_time:" + result.create_time.ToLongDateString());
            Debug.WriteLine("id:" + result.id);
            Debug.WriteLine("intend:" + result.intent);
            Debug.WriteLine("link 0 - href:" + result.links[0].href);
            Debug.WriteLine("link 0 - method:" + result.links[0].method);
            Debug.WriteLine("link 0 - rel:" + result.links[0].rel);
            Debug.WriteLine("payer_info - first_name:" + result.payer.payer_info.first_name);
            Debug.WriteLine("payer_info - last_name:" + result.payer.payer_info.last_name);
            Debug.WriteLine("payer_info - email:" + result.payer.payer_info.email);
            Debug.WriteLine("payer_info - blilling_address:" + result.payer.payer_info.billing_address);
            Debug.WriteLine("payer_info - country_code:" + result.payer.payer_info.country_code);
            Debug.WriteLine("payer_info - shipping_address:" + result.payer.payer_info.shipping_address);
            Debug.WriteLine("payer_info - payer_id:" + result.payer.payer_info.payer_id);
            Debug.WriteLine("state:" + result.state);

            ViewBag.name = result.payer.payer_info.first_name;
            return View("Success");
        }
    }
}