namespace ConvertDanhSachTinhThanh.Constants
{
    /// <summary>
    /// Hằng số dùng chung của ứng dụng: loại địa chỉ, câu truy vấn danh mục mới, endpoint API và thông báo.
    /// </summary>
    public static class AppConstants
    {
        // Bốn loại địa chỉ hiển thị trên ComboBox. Giá trị phải khớp chính xác (kể cả hoa/thường) với
        // AddressQueryBuilder.Build và tham số @Loai của spd_InsertDanhSachDiaChiMoi trong Sql/Sql_procedure.sql.
        public const string ADDRESS_TYPE_PERMANENT = "Địa chỉ thường trú";
        public const string ADDRESS_TYPE_CURRENT = "Nơi ở hiện nay";
        public const string ADDRESS_TYPE_BIRTH = "Nơi sinh";
        public const string ADDRESS_TYPE_HOMETOWN = "Quê quán";

        // Câu truy vấn danh mục. GCRecord IS NULL loại các bản ghi đã xóa mềm.
        public const string DB_QUERY_TINH_THANH = "Select * from TinhThanh WHERE GCRecord IS NULL";
        public const string DB_QUERY_TINH_THANH_NEW = "Select Oid,MaQuanLy,TenTinhThanh from TinhThanh_New WHERE GCRecord IS NULL";
        public const string DB_QUERY_XA_PHUONG_NEW = "Select Oid,MaQuanLy,TenXaPhuong,TinhThanh_New from XaPhuong_New WHERE GCRecord IS NULL";

        // API chuyển một chuỗi địa chỉ đầy đủ sang địa chỉ mới (dùng cho địa chỉ thiếu cấp).
        public const string API_ENDPOINT_CONVERT_ADDRESS = "https://production.cas.so/address-kit/convert";
        // API chuyển theo mã tỉnh/huyện/xã cũ (dùng cho địa chỉ đủ 3 cấp), cần Bearer token.
        public const string API_ENDPOINT_CONVERT_3CAP = "https://tinhthanhpho.com/api/v1/convert/address";
        // Token không nên nằm trong mã nguồn; nên chuyển ra cấu hình không commit (xem docs/KNOWN_ISSUES.md).
        public const string API_AUTH_TOKEN = "hvn_u0bexhuCKIxin0sDBrbMe1Rf4d4m8a3c";

        // Các thông báo dưới đây hiện chưa được dùng: Form1 vẫn viết chuỗi trực tiếp.
        public const string ERROR_MISSING_CONNECTION = "Phải có chuỗi kết nối đến cơ sở dữ liệu trước";
        public const string ERROR_CONNECTION_FAILED = "❌ Connection failed: ";
        public const string ERROR_QUERY_FAILED = "❌ Error: ";
        public const string SUCCESS_MESSAGE = "Thành công";

        // Số địa chỉ mỗi lô khi ghi vào DB. Hiện chưa dùng: button5_Click viết cứng 100.
        public const int BATCH_SIZE = 100;
    }
}
