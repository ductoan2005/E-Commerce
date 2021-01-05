using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn1.Models
{
    public class DataContext:IdentityDbContext<KhachHang>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
        public DbSet<Laptop> Laptop { get; set; }
        public DbSet<ThuongHieu> ThuongHieu { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<HoaDon> HoaDon { get; set; }
        public DbSet<CT_HoaDon> CT_HoaDon { get; set; }
        public DbSet<LichSuMuaHang> LichSuMuaHang { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<GioHang> GioHang { get; set; }
        public DbSet<GhiChu> GhiChu { get; set; }
        public DbSet<Voucher> Voucher { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                },
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
                );
        }
    }
    public class GioHang 
    {
        [Key]
        public int GioHangID { get; set; }
        public int SoLuong { get; set; }
        public KhachHang KhachHang { get; set; }
        public string KhachHangID { get; set; }
        public Laptop Laptop { get; set; }
        public Nullable<int> LaptopID { get; set; }
        public ICollection<CT_HoaDon> CT_HoaDons { get; set; }
    }
    public class Laptop
    {
        [Key]
        public int LaptopID { get; set; }
        public string TenLaptop { get; set; }
        public int Giatien { get; set; }
        public string ThongSo { get; set; }
        public string HinhAnh { get; set; }
        public DateTime NgayTao { get; set; }
        public string GioiThieu { get; set; }
        public int SoLuong { get; set; }
        public virtual ThuongHieu ThuongHieu { get; set; }
        public Nullable<int> ThuongHieuID { get; set; }
        public virtual ICollection<LichSuMuaHang> LichSuMuaHangs { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<GioHang> GioHangs { get; set; }
    }
    public class ThuongHieu
    {
        [Key]
        public int ThuongHieuID { get; set; }
        public string TenThuongHieu { get; set; }
    }
    public class KhachHang: IdentityUser
    {
        public string TenKH { get; set; }
        public string DiaChiKH { get; set; }
        public string DienThoaiKH { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string HinhAnh { get; set; }
        public double TichDiem { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
    public class Voucher
    {
        [Key]
        public int VoucherID { get; set; }
        public string MaVoucher { get; set; }
        public int Discount { get; set; }
        public virtual KhachHang KhachHang { get; set; }
        public string KhachHangID { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayHetHan { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
    public class HoaDon
    {
        [Key]
        public int HoaDonID { get; set; }
        public DateTime Ngay { get; set; }  
        public virtual KhachHang KhachHang { get; set; }
        public string KhachHangID { get; set; }
        public virtual GhiChu GhiChu { get; set; }
        public Nullable<int> GhiChuID { get; set; }
        public int TrangThai { get; set; }
        public double TongTien { get; set; }
        public virtual Voucher  Voucher{ get; set; }
        public Nullable<int> VoucherID { get; set; }
        public virtual ICollection<LichSuMuaHang> LichSuMuaHangs { get; set; }
        public virtual ICollection<CT_HoaDon> CT_HoaDons { get; set; }
    }
    public class GhiChu
    {
        [Key]
        public int GhiChuID { get; set; }
        public string TenGhiChu { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
    public class CT_HoaDon
    {
        [Key]
        public int CT_HoaDonID { get; set; }
        public int SoLuong { get; set; }
        public int TongTien { get; set; }
        public DateTime Ngay { get; set; }
        public virtual Laptop Laptop { get; set; }
        public Nullable<int> LaptopID { get; set; }
        public virtual HoaDon HoaDon { get; set; }
        public Nullable<int> HoaDonID { get; set; }
        public virtual GioHang GioHang { get; set; }
        public Nullable<int> GioHangID { get; set; }
        
    }
    public class LichSuMuaHang
    {
        [Key]
        public int LichSuMuaHangID { get; set; }
        public virtual HoaDon HoaDon { get; set; }
        public Nullable<int> HoaDonID { get; set; }
    }
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }
        public virtual Laptop Laptop { get; set; }
        public Nullable<int> LaptopID { get; set; }
        public string Text { get; set; }
        public DateTime CmtDate { get; set; }
        public int Rating { get; set; }       
        public virtual KhachHang KhachHang { get; set; }
        public string KhachHangID { get; set; }
    }
}
