using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
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
