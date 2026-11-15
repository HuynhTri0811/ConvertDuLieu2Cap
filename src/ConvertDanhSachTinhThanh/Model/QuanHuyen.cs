using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{

    [XmlRoot("provinces")]
    public class QuanHuyen
    {
        public string code { get; set; }
        public string name { get; set; }
        public string englistName { get; set; }
        public string administrativeLevel { get; set; }
        public string provinceCode { get; set; }
        public string provinceName { get; set; }
        public string decree { get; set; }
    }


    [XmlRoot("DonVi")]
    public class QuanHuyenCu
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenDonViCha { get; set; }
        public string MaTen { get; set; }
        public string MaTenDonViCha { get; set; }
    }


    [XmlRoot("DonVi")]
    public class XaPhuongCu
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenDonViCha { get; set; }
        public string MaTen { get; set; }
        public string MaTenDonViCha { get; set; }
    }
}
