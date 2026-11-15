namespace ConvertDanhSachTinhThanh.Constants
{
    public static class AppConstants
    {
        public const string ADDRESS_TYPE_PERMANENT = "Địa chỉ thường trú";
        public const string ADDRESS_TYPE_CURRENT = "Nơi ở hiện nay";
        public const string ADDRESS_TYPE_BIRTH = "Nơi sinh";
        public const string ADDRESS_TYPE_HOMETOWN = "Quê quán";

        public const string DB_QUERY_TINH_THANH = "Select * from TinhThanh WHERE GCRecord IS NULL";
        public const string DB_QUERY_TINH_THANH_NEW = "Select Oid,MaQuanLy,TenTinhThanh from TinhThanh_New WHERE GCRecord IS NULL";
        public const string DB_QUERY_XA_PHUONG_NEW = "Select Oid,MaQuanLy,TenXaPhuong,TinhThanh_New from XaPhuong_New WHERE GCRecord IS NULL";

        public const string API_ENDPOINT_CONVERT_ADDRESS = "https://production.cas.so/address-kit/convert";
        public const string API_ENDPOINT_CONVERT_3CAP = "https://tinhthanhpho.com/api/v1/convert/address";
        public const string API_AUTH_TOKEN = "hvn_u0bexhuCKIxin0sDBrbMe1Rf4d4m8a3c";

        public const string ERROR_MISSING_CONNECTION = "Phải có chuỗi kết nối đến cơ sở dữ liệu trước";
        public const string ERROR_CONNECTION_FAILED = "❌ Connection failed: ";
        public const string ERROR_QUERY_FAILED = "❌ Error: ";
        public const string SUCCESS_MESSAGE = "Thành công";

        public const int BATCH_SIZE = 100;
    }
}
