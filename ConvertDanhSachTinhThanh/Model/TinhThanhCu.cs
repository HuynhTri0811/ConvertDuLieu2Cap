using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{
    [XmlRoot("provinces")]
    public class TinhThanhCu
    {
        public string province_id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }

    }
}