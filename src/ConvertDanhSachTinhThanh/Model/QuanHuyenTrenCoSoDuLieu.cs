using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Một dòng của bảng QuanHuyen cũ trong DB. Hiện chưa được nạp hay sử dụng.
    /// </summary>
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
