using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Một dòng của bảng XaPhuong_New (danh mục xã/phường mới đã nạp vào DB).
    /// </summary>
    public class XaPhuong_New
    {
        public Guid Oid { get; set; }
        public string MaQuanLy { get; set; }
        public string TenXaPhuong_New { get; set; }
        /// <summary>
        /// Oid của tỉnh mới chứa xã/phường này.
        /// </summary>
        public Guid TinhThanh_New { get; set; }

        public XaPhuong_New(Guid Oid,string MaQuanLy,string TenXaPhuong_New,Guid TinhThanh_New)
        {
            this.Oid = Oid;
            this.MaQuanLy = MaQuanLy;
            this.TenXaPhuong_New = TenXaPhuong_New;
            this.TinhThanh_New = TinhThanh_New;
        }

        /// <summary>
        /// DataGridView hiển thị đối tượng bằng ToString, nên trả về tên xã/phường.
        /// </summary>
        public override string ToString()
        {
            return TenXaPhuong_New;
        }
    }
}
