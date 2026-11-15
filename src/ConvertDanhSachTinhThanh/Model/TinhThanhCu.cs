using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Một tỉnh CŨ trong DanhSachTinhThanhCu.xml. name viết liền không dấu cách (ví dụ "AnGiang"),
    /// code là mã tỉnh cũ (ví dụ "89").
    /// </summary>
    [XmlRoot("provinces")]
    public class TinhThanhCu
    {
        public string province_id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }

    }
}