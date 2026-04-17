using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    public class TinhThanh_New
    {
        public Guid Oid { get; set; }
        public string MaQuanLy { get; set; }
        public string TenTinhThanh_New { get; set; }


        public TinhThanh_New()
        {

        }

        public TinhThanh_New(Guid Oid,string MaQuanLy,string TenTinhThanh_New)
        {
            this.Oid = Oid;
            this.MaQuanLy = MaQuanLy;
            this.TenTinhThanh_New = TenTinhThanh_New;
        }

        public override string ToString()
        {
            return TenTinhThanh_New;
        }
    }
}
