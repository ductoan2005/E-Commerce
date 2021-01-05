using AutoMapper;
using DoAn1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn1.Helper
{
    public class MapHelper : Profile
    {
        public MapHelper()
        {
            CreateMap<KhachHangDangKiModel, KhachHang>();
            CreateMap<KhachHangDangNhapModel, KhachHang>();
            CreateMap<AdminKhachHangDangKiModel, KhachHang>();
        }
    }
}
