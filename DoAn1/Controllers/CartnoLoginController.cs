using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DoAn1.EmailSender;
using DoAn1.Helper;
using DoAn1.Models;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn1.PaypalHelper;
using System.Diagnostics;

namespace DoAn1.Controllers
{
    public class CartnoLoginController : Controller
    {
        public IConfiguration configuration { get; }
        IEmailSender emailsender;
        DataContext context;
        public CartnoLoginController(IConfiguration configuration,IEmailSender emailsender, DataContext context)
        {
            this.emailsender = emailsender;
            this.context = context;
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
            List<LatopToCart> cart = SessionHelper.GetObjectFromJson<List<LatopToCart>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].laptop.LaptopID == id)
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
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<LatopToCart>>(HttpContext.Session, "cart");
            if (cart == null) return View("NotFound");
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(a => a.laptop.Giatien * a.SoLuong + 15);
            return View();
        }
        [HttpPost]

        public IActionResult Buy(int id, int soLuong)
        {

            if (SessionHelper.GetObjectFromJson<List<LatopToCart>>(HttpContext.Session, "cart") == null)
            {
                List<LatopToCart> cart = new List<LatopToCart>();
                cart.Add(new LatopToCart { laptop = context.Laptop.FirstOrDefault(p => p.LaptopID == id), SoLuong = soLuong });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<LatopToCart> cart = SessionHelper.GetObjectFromJson<List<LatopToCart>>(HttpContext.Session, "cart");
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
                    cart.Add(new LatopToCart { laptop = context.Laptop.FirstOrDefault(p => p.LaptopID == id), SoLuong = soLuong });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult CustomerDetails()
        {
            var cart2 = SessionHelper.GetObjectFromJson<List<LatopToCart>>(HttpContext.Session, "cart");
            if (cart2 == null) return View("NotFound");
            ViewBag.cart = cart2;
            ViewBag.total = cart2.Sum(a => a.laptop.Giatien * a.SoLuong);

            GioHang cart = new GioHang();
            return View(cart);
        }
        [HttpPost]
        public IActionResult CustomerDetails(string email, string sodienthoai, string diachi, string tenkhachhang)
        {
            List<LatopToCart> cart = SessionHelper.GetObjectFromJson<List<LatopToCart>>(HttpContext.Session, "cart");
            if (ModelState.IsValid)
            {
                if(context.KhachHang.Where(x=>x.Id == sodienthoai).Count()==0)
                {
                    KhachHang khachhang = new KhachHang()
                    {
                        Id = sodienthoai,
                        Email = email,
                        TenKH = tenkhachhang,
                        DiaChiKH = diachi,
                        DienThoaiKH = sodienthoai,
                    };
                    context.KhachHang.Add(khachhang);
                    context.SaveChanges();
                }
                    
                HoaDon hoaDon = new HoaDon()
                {
                    KhachHangID = sodienthoai,
                    Ngay = DateTime.Now,
                    TrangThai = 1
                };
                context.HoaDon.Add(hoaDon);
                context.SaveChanges();
                foreach (var item in cart)
                {
                    CT_HoaDon ct_HoaDon = new CT_HoaDon()
                    {
                        Ngay = DateTime.Now,
                        HoaDonID = hoaDon.HoaDonID,
                        LaptopID = item.laptop.LaptopID,
                        SoLuong = item.SoLuong,
                        TongTien = (item.laptop.Giatien * item.SoLuong) + 15
                    };
                    Laptop lt = context.Laptop.Find(ct_HoaDon.LaptopID);
                    lt.SoLuong = lt.SoLuong - item.SoLuong;
                    context.Entry(lt).State = EntityState.Modified;
                    context.CT_HoaDon.Add(ct_HoaDon);
                    context.SaveChanges();
                }
                cart.Clear();
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
                SendEmail(sodienthoai);
                return View("CheckOut");
                
            }
            
            else return View();
        }
        public IActionResult CheckOut()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Remove(int id)
        {
            List<LatopToCart> cart = SessionHelper.GetObjectFromJson<List<LatopToCart>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult SendEmail(string id)
        {
            var emailcustomer = context.KhachHang.Find(id).Email;

            var message = new Message(new string[] { emailcustomer}, "", "");
            message.Subject = "Thanks for buying!, Vui lòng kiểm tra lại hoá đơn của quý khách!";
            message.Content = "Quý khách vui lòng kiểm tra quá trình vận chuyển sản phẩm trong phần tìm đơn hàng";
            emailsender.SendEmail(message);

            return View("CheckOut");
        }
        [HttpPost]
        public async Task<IActionResult> Checkoutpaypal(double total)
        {
            var payPalAPI = new PayPalAPI(configuration);
            string url = await payPalAPI.getRedirectURLToPayPal(total);
            return Redirect(url);
        }
        [Route("success")]
        public async Task<IActionResult> Success([FromQuery(Name = "paymentId")] string paymentId, [FromQuery(Name = "PayerID")] string payerID)
        {
            var payPalAPI = new PayPalAPI(configuration);
            PayPalPaymentExecutedResponse result = await payPalAPI.executedPayment(paymentId, payerID);
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
        public IActionResult Success()
        {
            return View();
        }
    }
}