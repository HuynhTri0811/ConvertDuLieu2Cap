using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Một tỉnh MỚI trong DanhSachTinhThanhMoi.xml; code là mã quản lý của tỉnh.
    /// </summary>
    [XmlRoot("provinces")]
    public class TinhThanh
    {
        public string code { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public string administrativeLevel { get; set; }
        public string decree { get; set; }

    }
}
