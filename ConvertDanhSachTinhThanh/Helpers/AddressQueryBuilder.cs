using ConvertDanhSachTinhThanh.Constants;

namespace ConvertDanhSachTinhThanh.Helpers
{
    /// <summary>
    /// Builds the address lookup query for the selected address field.
    /// </summary>
    public static class AddressQueryBuilder
    {
        private const string BaseQuery =
            "SELECT hs.Oid AS ThongTinNhanVien_Oid, dc.FullDiaChi, " +
            "(SELECT TOP 1 TenTinhThanh FROM TinhThanh tt WHERE tt.Oid = dc.TinhThanh) AS TinhThanh, " +
            "(SELECT TOP 1 TenQuanHuyen FROM QuanHuyen qh WHERE qh.Oid = dc.QuanHuyen) AS QuanHuyen, " +
            "(SELECT TOP 1 TenXaPhuong FROM XaPhuong xp WHERE xp.Oid = dc.XaPhuong) AS XaPhuong, dc.SoNha " +
            "FROM HoSo hs JOIN DiaChi dc ON {0} " +
            "WHERE hs.GCRecord IS NULL AND ISNULL(dc.FullDiaChi, '') <> ''";

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
