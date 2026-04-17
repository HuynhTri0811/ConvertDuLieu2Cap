using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    public class TinhThanhTrenCoSoDuLieu
    {
        public Guid Oid { get; set; }
        public string MaQuanLy { get; set; }
        public string TenTinhThanh { get; set; }

        public TinhThanhTrenCoSoDuLieu(Guid Oid,string MaQuanLy, string TenTinhThanh)
        {
            this.Oid = Oid;
            this.MaQuanLy = MaQuanLy;
            this.TenTinhThanh = TenTinhThanh;
        }
    }
}
