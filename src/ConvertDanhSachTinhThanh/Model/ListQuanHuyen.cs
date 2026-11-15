using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Gốc của Data/DanhSachMoi/DanhSachQuanHuyenMoi.xml. Dù tên là quận/huyện,
    /// nội dung là danh sách XÃ/PHƯỜNG mới (3.321 bản ghi).
    /// </summary>
    [XmlRoot("root")]
    public class ListQuanHuyen
    {
        [XmlElement("communes")]
        public List<QuanHuyen> Items { get; set; }

    }


    /// <summary>
    /// Gốc của Data/DanhSachCu/DanhSachQuanHuyenCu.xml: quận/huyện cũ.
    /// </summary>
    [XmlRoot("DanhSachDonVi")]
    public class ListQuanHuyenCu
    {
        [XmlElement("DonVi")]
        public List<QuanHuyenCu> Items { get; set; }

    }


    /// <summary>
    /// Gốc của Data/DanhSachCu/XaPhuongCu.xml: xã/phường cũ.
    /// </summary>
    [XmlRoot("DanhSachDonVi")]
    public class ListXaPhuongCu
    {
        [XmlElement("DonVi")]
        public List<XaPhuongCu> Items { get; set; }

    }
}
