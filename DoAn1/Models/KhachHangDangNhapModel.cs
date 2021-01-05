using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn1.Models
{
    public class KhachHangDangNhapModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnURL { get; set; }
        public IList<AuthenticationScheme> ExternalLogin { get; set; }
    }
}
