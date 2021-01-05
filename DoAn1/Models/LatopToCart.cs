using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn1.Models
{
    public class LatopToCart
    {
        public Laptop laptop { get; set; }
        public KhachHang khachhang { get; set; }
        public int SoLuong { get; set; }
        public override int GetHashCode()
        {
            return laptop.LaptopID;
        }
    }
}
