using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Kết quả chuyển đổi của một hồ sơ, hiển thị ở dataGridView2 và được ghi vào DB ở bước "Insert vào địa chỉ mới".
    /// TinhThanhNew/XaPhuong_New là null khi API lỗi hoặc không tra được trong danh mục mới.
    /// </summary>
    public class DiaChiMoi
    {
        public Guid HoSo { get; set; }
        public TinhThanh_New TinhThanhNew { get; set; }
        public XaPhuong_New XaPhuong_New { get; set; }
        public string FullDiaChi { get; set; }

        public DiaChiMoi(Guid Oid_,string FullDiaChi_)
        {
            this.HoSo = Oid_;
            this.FullDiaChi = FullDiaChi_;
        }
        public DiaChiMoi()
        {

        }

        




    }


}
