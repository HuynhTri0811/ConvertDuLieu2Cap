using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    public class XaPhuong_New
    {
        public Guid Oid { get; set; }
        public string MaQuanLy { get; set; }
        public string TenXaPhuong_New { get; set; }
        public Guid TinhThanh_New { get; set; }

        public XaPhuong_New(Guid Oid,string MaQuanLy,string TenXaPhuong_New,Guid TinhThanh_New)
        {
            this.Oid = Oid;
            this.MaQuanLy = MaQuanLy;
            this.TenXaPhuong_New = TenXaPhuong_New;
            this.TinhThanh_New = TinhThanh_New;
        }

        public override string ToString()
        {
            return TenXaPhuong_New;
        }
    }
}
