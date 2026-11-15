using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Gốc của Data/DanhSachMoi/DanhSachTinhThanhMoi.xml: danh sách tỉnh mới (34 tỉnh).
    /// Tên lớp viết sai chính tả (Lisst) nhưng được giữ để không phải sửa nơi gọi.
    /// </summary>
    [XmlRoot("root")]
    public class LisstTinhThanh
    {
        [XmlElement("provinces")]
        public List<TinhThanh> Items { get; set; }

    }

    /// <summary>
    /// Gốc của Data/DanhSachCu/DanhSachTinhThanhCu.xml: danh sách tỉnh cũ (63 tỉnh).
    /// </summary>
    [XmlRoot("root")]
    public class LisstTinhThanhCu
    {
        [XmlElement("data")]
        public List<TinhThanhCu> Items { get; set; }

    }

}
