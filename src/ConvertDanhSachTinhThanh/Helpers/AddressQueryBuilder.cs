using ConvertDanhSachTinhThanh.Constants;

namespace ConvertDanhSachTinhThanh.Helpers
{
    /// <summary>
    /// Dựng câu SELECT đọc địa chỉ cũ của hồ sơ theo loại địa chỉ được chọn trên ComboBox.
    /// </summary>
    public static class AddressQueryBuilder
    {
        // Phần SELECT dùng chung; {0} là điều kiện JOIN giữa HoSo và DiaChi theo loại địa chỉ.
        // Các cột trả về được Form1 đọc theo tên: ThongTinNhanVien_Oid, FullDiaChi, TinhThanh, QuanHuyen, XaPhuong, SoNha.
        private const string BaseQuery =
            "SELECT hs.Oid AS ThongTinNhanVien_Oid, dc.FullDiaChi, " +
            "(SELECT TOP 1 TenTinhThanh FROM TinhThanh tt WHERE tt.Oid = dc.TinhThanh) AS TinhThanh, " +
            "(SELECT TOP 1 TenQuanHuyen FROM QuanHuyen qh WHERE qh.Oid = dc.QuanHuyen) AS QuanHuyen, " +
            "(SELECT TOP 1 TenXaPhuong FROM XaPhuong xp WHERE xp.Oid = dc.XaPhuong) AS XaPhuong, dc.SoNha " +
            "FROM HoSo hs JOIN DiaChi dc ON {0} " +
            "WHERE hs.GCRecord IS NULL AND ISNULL(dc.FullDiaChi, '') <> ''";

        /// <summary>
        /// Trả về câu truy vấn cho một loại địa chỉ trong AppConstants.ADDRESS_TYPE_*.
        /// </summary>
        /// <param name="addressType">Text đang chọn trên ComboBox; so khớp chính xác, phân biệt hoa/thường.</param>
        /// <returns>
        /// Câu SQL, hoặc null nếu loại địa chỉ không hợp lệ. Giá trị đầu vào không bao giờ được nối vào SQL;
        /// chỉ các điều kiện JOIN cố định trong hàm được dùng.
        /// </returns>
        public static string Build(string addressType)
        {
            string relationship;

            if (addressType == AppConstants.ADDRESS_TYPE_PERMANENT)
                relationship = "hs.DiaChiThuongTru = dc.Oid";
            else if (addressType == AppConstants.ADDRESS_TYPE_CURRENT)
                relationship = "hs.NoiOHienNay = dc.Oid";
            else if (addressType == AppConstants.ADDRESS_TYPE_BIRTH)
                relationship = "hs.NoiSinh = dc.Oid";
            else if (addressType == AppConstants.ADDRESS_TYPE_HOMETOWN)
                relationship = "hs.QueQuan = dc.Oid";
            else
                return null;

            return string.Format(BaseQuery, relationship);
        }
    }
}
