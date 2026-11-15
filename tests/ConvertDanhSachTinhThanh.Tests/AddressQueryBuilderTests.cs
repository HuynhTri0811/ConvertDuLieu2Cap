using ConvertDanhSachTinhThanh.Constants;
using ConvertDanhSachTinhThanh.Helpers;
using Xunit;

namespace ConvertDanhSachTinhThanh.Tests;

public class AddressQueryBuilderTests
{
    [Theory]
    [InlineData(AppConstants.ADDRESS_TYPE_PERMANENT, "hs.DiaChiThuongTru = dc.Oid")]
    [InlineData(AppConstants.ADDRESS_TYPE_CURRENT, "hs.NoiOHienNay = dc.Oid")]
    [InlineData(AppConstants.ADDRESS_TYPE_BIRTH, "hs.NoiSinh = dc.Oid")]
    [InlineData(AppConstants.ADDRESS_TYPE_HOMETOWN, "hs.QueQuan = dc.Oid")]
    public void Build_KnownAddressType_JoinsOnMatchingColumn(string addressType, string expectedJoin)
    {
        string query = AddressQueryBuilder.Build(addressType);

        Assert.Contains("JOIN DiaChi dc ON " + expectedJoin + " ", query);
    }

    [Theory]
    [InlineData(AppConstants.ADDRESS_TYPE_PERMANENT)]
    [InlineData(AppConstants.ADDRESS_TYPE_CURRENT)]
    [InlineData(AppConstants.ADDRESS_TYPE_BIRTH)]
    [InlineData(AppConstants.ADDRESS_TYPE_HOMETOWN)]
    public void Build_KnownAddressType_SelectsSharedColumnsAndFilters(string addressType)
    {
        string query = AddressQueryBuilder.Build(addressType);

        Assert.StartsWith("SELECT hs.Oid AS ThongTinNhanVien_Oid, dc.FullDiaChi, ", query);
        Assert.Contains("AS TinhThanh", query);
        Assert.Contains("AS QuanHuyen", query);
        Assert.Contains("AS XaPhuong", query);
        Assert.Contains("dc.SoNha", query);
        Assert.EndsWith("WHERE hs.GCRecord IS NULL AND ISNULL(dc.FullDiaChi, '') <> ''", query);
    }

    [Fact]
    public void Build_EachAddressType_ProducesDistinctQuery()
    {
        string[] queries =
        {
            AddressQueryBuilder.Build(AppConstants.ADDRESS_TYPE_PERMANENT),
            AddressQueryBuilder.Build(AppConstants.ADDRESS_TYPE_CURRENT),
            AddressQueryBuilder.Build(AppConstants.ADDRESS_TYPE_BIRTH),
            AddressQueryBuilder.Build(AppConstants.ADDRESS_TYPE_HOMETOWN),
        };

        Assert.Equal(queries.Length, queries.Distinct().Count());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Địa chỉ tạm trú")]
    [InlineData("địa chỉ thường trú")] // Matching is case-sensitive.
    [InlineData("hs.QueQuan = dc.Oid; DROP TABLE HoSo --")]
    public void Build_UnknownAddressType_ReturnsNull(string? addressType)
    {
        Assert.Null(AddressQueryBuilder.Build(addressType!));
    }
}
