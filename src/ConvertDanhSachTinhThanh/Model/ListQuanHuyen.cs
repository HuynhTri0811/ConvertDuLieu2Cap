using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{
    [XmlRoot("root")]
    public class ListQuanHuyen
    {
        [XmlElement("communes")]
        public List<QuanHuyen> Items { get; set; }

    }


    [XmlRoot("DanhSachDonVi")]
    public class ListQuanHuyenCu
    {
        [XmlElement("DonVi")]
        public List<QuanHuyenCu> Items { get; set; }

    }


    [XmlRoot("DanhSachDonVi")]
    public class ListXaPhuongCu
    {
        [XmlElement("DonVi")]
        public List<XaPhuongCu> Items { get; set; }

    }
}
