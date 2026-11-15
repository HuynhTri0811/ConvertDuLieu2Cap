using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{
    [XmlRoot("root")]
    public class LisstTinhThanh
    {
        [XmlElement("provinces")]
        public List<TinhThanh> Items { get; set; }

    }

    [XmlRoot("root")]
    public class LisstTinhThanhCu
    {
        [XmlElement("data")]
        public List<TinhThanhCu> Items { get; set; }

    }

}
