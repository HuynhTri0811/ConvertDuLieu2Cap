using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDanhSachTinhThanh
{
    /// <summary>
    /// Một địa chỉ cũ (3 cấp) của hồ sơ, đọc từ bảng HoSo/DiaChi ở bước "Load danh sách địa chỉ cũ".
    /// </summary>
    public class DiaChiCu
    {
        /// <summary>
        /// Oid của hồ sơ (HoSo.Oid) sở hữu địa chỉ.
        /// </summary>
        public Guid HoSo { get; set; }
        /// <summary>
        /// Tỉnh, huyện, xã cũ. Ban đầu là tên đọc từ DB; khi nạp xong được đổi thành MÃ trong danh mục cũ
        /// (Data/DanhSachCu) nếu tìm thấy, nếu không thì giữ tên đã chuẩn hóa. API 3 cấp nhận các mã này.
        /// </summary>
        public string TenTinhThanhCu { get; set; }
        public string TenQuanHuyenCu { get; set; }
        public string TenXaPhuongCu { get; set; }
        /// <summary>
        /// Số nhà/đường, gửi kèm cho API 3 cấp và chép sang địa chỉ mới.
        /// </summary>
        public string SoNha { get; set; }
        /// <summary>
        /// Địa chỉ đầy đủ dạng chuỗi, gửi cho API khi địa chỉ thiếu cấp.
        /// </summary>
        public string FullDiaChi { get; set; }

        public DiaChiCu(Guid Oid_,string FullDiaChi_,string TenTinhThanhCu_,string TenQuanHuyenCu_,string TenXaPhuongCu_)
        {
            this.HoSo = Oid_;
            this.FullDiaChi = FullDiaChi_;
            this.TenTinhThanhCu = TenTinhThanhCu_;
            this.TenQuanHuyenCu = TenQuanHuyenCu_;
            this.TenXaPhuongCu = TenXaPhuongCu_;
        }
    }



    /// <summary>
    /// Địa chỉ mới do API địa chỉ đầy đủ (API_ENDPOINT_CONVERT_ADDRESS) trả về.
    /// </summary>
    public class NewAddressDTO
    {
        /// <summary>
        /// Mã xã/phường mới, tra trong XaPhuong_New.MaQuanLy.
        /// </summary>
        public string Code { get; set; }
        public string Commune { get; set; }
        /// <summary>
        /// Tên tỉnh mới, tra trong TinhThanh_New theo tên.
        /// </summary>
        public string Province { get; set; }
        public string FullAddress { get; set; }
    }


    /// <summary>
    /// Phản hồi của API địa chỉ đầy đủ.
    /// </summary>
    public class ApiResponseDto
    {
        public NewAddressDTO NewAddress { get; set; }
    }


    /// <summary>
    /// Request gửi API địa chỉ đầy đủ. Tên thuộc tính viết thường vì phải khớp JSON của API.
    /// </summary>
    public class RequestModel
    {
        public string oldAddress { get; set; }
    }


    /// <summary>
    /// Request gửi API 3 cấp: mã tỉnh, huyện, xã cũ và số nhà. Tên thuộc tính khớp JSON của API.
    /// </summary>
    public class RequesModel_3Cap
    {
        public string provinceCode { get; set; }
        public string districtCode { get; set; }
        public string wardCode { get; set; }
        public string streetAddress { get; set; }
    }
    /// <summary>
    /// Phần data của phản hồi API 3 cấp: địa chỉ cũ, địa chỉ mới và ghi chú sáp nhập.
    /// </summary>
    public class Data
    {
        public Old old { get; set; }
        /// <summary>
        /// Địa chỉ mới. Dùng @new vì new là từ khóa C#.
        /// </summary>
        public New @new { get; set; }
        public MergeInfo mergeInfo { get; set; }
    }

    /// <summary>
    /// Quận/huyện trong phản hồi API 3 cấp (chỉ có ở địa chỉ cũ).
    /// </summary>
    public class District
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    /// <summary>
    /// Ghi chú về việc sáp nhập đơn vị hành chính.
    /// </summary>
    public class MergeInfo
    {
        public string notes { get; set; }
    }

    /// <summary>
    /// Địa chỉ mới 2 cấp trong phản hồi API 3 cấp.
    /// </summary>
    public class New
    {
        public Province province { get; set; }
        public Ward ward { get; set; }
        public string fullAddress { get; set; }
    }

    /// <summary>
    /// Địa chỉ cũ 3 cấp mà API 3 cấp đã nhận diện.
    /// </summary>
    public class Old
    {
        public Province province { get; set; }
        public District district { get; set; }
        public Ward ward { get; set; }
        public string fullAddress { get; set; }
    }

    /// <summary>
    /// Tỉnh/thành trong phản hồi API 3 cấp; code là mã tỉnh, tra trong TinhThanh_New.MaQuanLy.
    /// </summary>
    public class Province
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    /// <summary>
    /// Gốc phản hồi của API 3 cấp. data là null khi success = false.
    /// </summary>
    public class Root
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    /// <summary>
    /// Xã/phường trong phản hồi API 3 cấp; code là mã xã, tra trong XaPhuong_New.MaQuanLy.
    /// </summary>
    public class Ward
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }


}
