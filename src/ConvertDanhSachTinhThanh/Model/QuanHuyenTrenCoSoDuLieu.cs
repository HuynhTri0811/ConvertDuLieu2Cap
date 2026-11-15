using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    public class QuanHuyenTrenCoSoDuLieu
    {
        public Guid Oid { get; set; }
        public string MaQuanLy { get; set; }
        public string TenQuanHuyen { get; set; }

        public TinhThanhTrenCoSoDuLieu TinhThanh { get; set; }

        public QuanHuyenTrenCoSoDuLieu(Guid Oid,string MaQuanLy, string TenQuanHuyen)
        {
            this.Oid = Oid;
            this.MaQuanLy = MaQuanLy;
            this.TenQuanHuyen = TenQuanHuyen;
        }
    }
}
