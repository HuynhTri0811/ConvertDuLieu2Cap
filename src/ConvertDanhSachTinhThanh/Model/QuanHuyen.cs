using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConvertDanhSachTinhThanh
{

    /// <summary>
    /// Một xã/phường MỚI trong DanhSachQuanHuyenMoi.xml (tên lớp là lịch sử, không phải quận/huyện).
    /// XmlRoot ở đây không có tác dụng: tên phần tử do [XmlElement("communes")] của ListQuanHuyen quyết định.
    /// </summary>
    [XmlRoot("provinces")]
    public class QuanHuyen
    {
        public string code { get; set; }
        public string name { get; set; }
        // Viết sai (XML dùng <englishName>) nên luôn null; hiện không nơi nào dùng.
        public string englistName { get; set; }
        public string administrativeLevel { get; set; }
        /// <summary>
        /// Mã tỉnh mới mà xã/phường thuộc về.
        /// </summary>
        public string provinceCode { get; set; }
        public string provinceName { get; set; }
        public string decree { get; set; }
    }


    /// <summary>
    /// Một quận/huyện cũ. Tên viết liền không dấu cách (ví dụ "BaĐình").
    /// MaTen có dạng "mã-tên-tên tỉnh", ví dụ "001-BaĐình-HàNội".
    /// </summary>
    [XmlRoot("DonVi")]
    public class QuanHuyenCu
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenDonViCha { get; set; }
        public string MaTen { get; set; }
        public string MaTenDonViCha { get; set; }
    }


    /// <summary>
    /// Một xã/phường cũ. MaTenDonViCha trỏ tới MaTen của huyện cha; Form1 chỉ so khớp phần mã.
    /// Khoảng 590 bản ghi ghi tên huyện cha khác danh mục huyện ("ThịNghiSơn" thay vì "NghiSơn").
    /// </summary>
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
