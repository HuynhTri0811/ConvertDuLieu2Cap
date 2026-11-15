using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using System.Xml.Serialization;
using ConvertDanhSachTinhThanh.Constants;

namespace ConvertDanhSachTinhThanh
{


    /// <summary>
    /// Màn hình chính. Lớp được chia thành nhiều file partial:
    /// Form1.cs (state dùng chung, nạp danh mục cũ), Form1.Database.cs (SQL Server),
    /// Form1.AddressConversion.cs (gọi API chuyển đổi), Form1.Designer.cs (giao diện do Designer sinh).
    /// Quy trình: nạp danh mục mới vào DB → nạp địa chỉ cũ → chuyển đổi → ghi địa chỉ mới (docs/ARCHITECTURE.md).
    /// </summary>
    public partial class Form1 : Form
    {
        // Một HttpClient dùng chung cho cả hai API, tạo một lần để tránh cạn socket.
        private static readonly HttpClient client = new HttpClient();

        // Kết quả chuyển đổi qua API 3 cấp (địa chỉ đủ tỉnh/huyện/xã).
        List<DiaChiMoi> DiaChiMoi_3Cap { get; set; }
        // Kết quả chuyển đổi qua API địa chỉ đầy đủ (địa chỉ thiếu cấp).
        List<DiaChiMoi> DiaChiMoi_DiaChiFull { get; set; }


        // Danh mục cũ 3 cấp, nạp từ Data/DanhSachCu khi mở form; dùng để đổi tên tỉnh/huyện/xã cũ sang mã.
        List<TinhThanhCu> listTinhThanhCu { get; set; }
        List<QuanHuyenCu> listQuanHuyenCu { get; set; }
        List<XaPhuongCu> listXaPhuongCu { get; set; }
        // Danh mục tỉnh mới, đọc từ bảng TinhThanh_New mỗi lần chuyển đổi.
        List<TinhThanh_New> listTinhThanhNew { get; set; }
        // Cache các tỉnh mới đã tra được; tìm ở đây trước khi duyệt cả listTinhThanhNew.
        List<TinhThanh_New> listTinhThanhNew_TonTai { get; set; }
        // Cache các xã/phường mới đã tra được.
        List<XaPhuong_New> ListXaPhuongNew_TonTai { get; set; }
        // Danh mục xã/phường mới, đọc từ bảng XaPhuong_New mỗi lần chuyển đổi.
        List<XaPhuong_New> ListXaPhuongNew { get; set; }
        // Tỉnh cũ trong bảng TinhThanh, nạp bởi nút "Convert Tỉnh Thành"; hiện chưa bước nào dùng tới.
        List<TinhThanhTrenCoSoDuLieu> listTinhThanh_CoTrenCoSoDuLieu { get; set; }
        // Chưa được nạp hay sử dụng.
        List<QuanHuyenTrenCoSoDuLieu> listQuanHuyen_CoTrenCoDuLieu { get; set; }
        // Địa chỉ cũ đã nạp (bước 2), đầu vào của bước chuyển đổi.
        List<DiaChiCu> listDiaChiCu { get; set; }
        // Địa chỉ mới sau chuyển đổi (bước 3), đầu vào của bước ghi vào DB.
        List<DiaChiMoi> listDiaChiMoi { get; set; }
        /// <summary>
        /// Khởi tạo giao diện, nạp ba danh mục cũ từ XML và điền ComboBox loại địa chỉ.
        /// Thiếu một file trong Data/DanhSachCu thì form ném lỗi ngay khi mở.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            DiaChiMoi_3Cap = new List<DiaChiMoi>();
            DiaChiMoi_DiaChiFull = new List<DiaChiMoi>();
            listTinhThanhCu = GetTinhThanhCu();
            listQuanHuyenCu = GetQuanHuyenCu();
            listXaPhuongCu = GetXaPhuongCu();
            listTinhThanh_CoTrenCoSoDuLieu = new List<TinhThanhTrenCoSoDuLieu>();
            listQuanHuyen_CoTrenCoDuLieu = new List<QuanHuyenTrenCoSoDuLieu>();
            listDiaChiCu = new List<DiaChiCu>();
            listDiaChiMoi = new List<DiaChiMoi>();
            AddDanhSachComboBox();

        }

        /// <summary>
        /// Đọc danh sách xã/phường cũ từ Data/DanhSachCu/XaPhuongCu.xml.
        /// </summary>
        private List<XaPhuongCu> GetXaPhuongCu()
        {
            string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DanhSachCu", "XaPhuongCu.xml");
            string xml = File.ReadAllText(debugPath);


            XmlSerializer serializer = new XmlSerializer(typeof(ListXaPhuongCu));
            using (StringReader reader = new StringReader(xml))
            {
                ListXaPhuongCu people = (ListXaPhuongCu)serializer.Deserialize(reader);
                return people.Items.Cast<XaPhuongCu>().ToList();
            }
        }

        /// <summary>
        /// Điền bốn loại địa chỉ. Text của ComboBox được truyền cho AddressQueryBuilder.Build
        /// và tham số @Loai của spd_InsertDanhSachDiaChiMoi, nên phải giữ nguyên giá trị trong AppConstants.
        /// </summary>
        public void AddDanhSachComboBox()
        {
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_PERMANENT);
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_CURRENT);
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_BIRTH);
            comboBox1.Items.Add(AppConstants.ADDRESS_TYPE_HOMETOWN);
        }

        /// <summary>
        /// Đọc danh sách tỉnh cũ từ Data/DanhSachCu/DanhSachTinhThanhCu.xml.
        /// </summary>
        private List<TinhThanhCu> GetTinhThanhCu()
        {
            string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DanhSachCu", "DanhSachTinhThanhCu.xml");
            string xml = File.ReadAllText(debugPath);


            XmlSerializer serializer = new XmlSerializer(typeof(LisstTinhThanhCu));
            using (StringReader reader = new StringReader(xml))
            {
                LisstTinhThanhCu people = (LisstTinhThanhCu)serializer.Deserialize(reader);
                return people.Items.Cast<TinhThanhCu>().ToList();
            }
        }

        /// <summary>
        /// Đọc danh sách quận/huyện cũ từ Data/DanhSachCu/DanhSachQuanHuyenCu.xml.
        /// </summary>
        private List<QuanHuyenCu> GetQuanHuyenCu()
        {
            string debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DanhSachCu", "DanhSachQuanHuyenCu.xml");
            string xml = File.ReadAllText(debugPath);
            XmlSerializer serializer = new XmlSerializer(typeof(ListQuanHuyenCu));
            using (StringReader reader = new StringReader(xml))
            {
                ListQuanHuyenCu people = (ListQuanHuyenCu)serializer.Deserialize(reader);
                return people.Items.Cast<QuanHuyenCu>().ToList();
            }
        }
    }
}
