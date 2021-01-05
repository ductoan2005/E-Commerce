using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn1.Models
{
    public class AdminKhachHangDangKiModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DiaChiKH { get; set; }
        public string DienThoaiKH { get; set; }
        public int TichDiem { get; set; }
        public DateTime NgaySinh { get; set; }
        public string HinhAnh { get; set; }
        public bool RememberMe { get; set; }
    }
}
