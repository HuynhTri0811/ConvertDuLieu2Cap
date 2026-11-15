using System.Xml.Serialization;
using Xunit;

namespace ConvertDanhSachTinhThanh.Tests;

/// <summary>
/// Loads the real catalog files shipped with the app, using the same model
/// classes and XmlSerializer approach as Form1, to catch data or mapping breakage.
/// </summary>
public class CatalogXmlTests
{
    private static T Load<T>(params string[] relativePath)
    {
        string path = Path.Combine(new[] { AppContext.BaseDirectory, "Data" }.Concat(relativePath).ToArray());
        using var reader = new StringReader(File.ReadAllText(path));
        return (T)new XmlSerializer(typeof(T)).Deserialize(reader)!;
    }

    // ---- Danh mục cũ (3 cấp) ----

    [Fact]
    public void TinhThanhCu_LoadsAll63OldProvinces()
    {
        var items = Load<LisstTinhThanhCu>("DanhSachCu", "DanhSachTinhThanhCu.xml").Items;

        Assert.Equal(63, items.Count);
        Assert.All(items, x =>
        {
            Assert.False(string.IsNullOrWhiteSpace(x.code));
            Assert.False(string.IsNullOrWhiteSpace(x.name));
        });
        Assert.Equal(63, items.Select(x => x.code).Distinct().Count());
    }

    [Fact]
    public void QuanHuyenCu_LoadsDistrictsWithParentProvince()
    {
        var items = Load<ListQuanHuyenCu>("DanhSachCu", "DanhSachQuanHuyenCu.xml").Items;

        Assert.Equal(707, items.Count);
        Assert.All(items, x =>
        {
            Assert.False(string.IsNullOrWhiteSpace(x.Ma));
            Assert.False(string.IsNullOrWhiteSpace(x.Ten));
            Assert.False(string.IsNullOrWhiteSpace(x.TenDonViCha));
        });

        var baDinh = Assert.Single(items, x => x.Ma == "001");
        Assert.Equal("BaĐình", baDinh.Ten);
        Assert.Equal("HàNội", baDinh.TenDonViCha);
        Assert.Equal("001-BaĐình-HàNội", baDinh.MaTen);
        Assert.Equal("01-HàNội", baDinh.MaTenDonViCha);
    }

    [Fact]
    public void XaPhuongCu_LoadsWardsWithParentDistrict()
    {
        var items = Load<ListXaPhuongCu>("DanhSachCu", "XaPhuongCu.xml").Items;

        Assert.Equal(10599, items.Count);
        Assert.All(items, x =>
        {
            Assert.False(string.IsNullOrWhiteSpace(x.Ma));
            Assert.False(string.IsNullOrWhiteSpace(x.Ten));
        });

        var vinhPhuc = Assert.Single(items, x => x.Ma == "00006");
        Assert.Equal("VĩnhPhúc", vinhPhuc.Ten);
        Assert.Equal("BaĐình", vinhPhuc.TenDonViCha);
    }

    [Fact]
    public void XaPhuongCu_EveryParentDistrictCodeExistsInDistrictCatalog()
    {
        // Form1 matches a ward to its district by the district code contained in
        // MaTenDonViCha (e.g. "001-BaĐình-HàNội"), so the code prefix must resolve.
        // Names are not compared: ~590 wards spell their "Thị xã" parent as
        // "Thị<Tên>" while the district catalog uses "<Tên>".
        var districtCodes = Load<ListQuanHuyenCu>("DanhSachCu", "DanhSachQuanHuyenCu.xml").Items
            .Select(x => x.Ma)
            .ToHashSet();
        var wards = Load<ListXaPhuongCu>("DanhSachCu", "XaPhuongCu.xml").Items;

        var orphans = wards
            .Where(x => !districtCodes.Contains(x.MaTenDonViCha.Split('-')[0]))
            .Select(x => x.MaTen)
            .ToList();

        Assert.Empty(orphans);
    }

    // ---- Danh mục mới (2 cấp) ----

    [Fact]
    public void TinhThanhMoi_Loads34NewProvinces()
    {
        var items = Load<LisstTinhThanh>("DanhSachMoi", "DanhSachTinhThanhMoi.xml").Items;

        Assert.Equal(34, items.Count);
        Assert.Equal(34, items.Select(x => x.code).Distinct().Count());

        var haNoi = Assert.Single(items, x => x.code == "01");
        Assert.Equal("Thành phố Hà Nội", haNoi.name);
        Assert.Equal("Thành phố Trung ương", haNoi.administrativeLevel);
    }

    [Fact]
    public void XaPhuongMoi_LoadsCommunesLinkedToNewProvinces()
    {
        var provinceCodes = Load<LisstTinhThanh>("DanhSachMoi", "DanhSachTinhThanhMoi.xml").Items
            .Select(x => x.code)
            .ToHashSet();
        var communes = Load<ListQuanHuyen>("DanhSachMoi", "DanhSachQuanHuyenMoi.xml").Items;

        Assert.Equal(3321, communes.Count);
        Assert.Equal(communes.Count, communes.Select(x => x.code).Distinct().Count());
        Assert.All(communes, x => Assert.Contains(x.provinceCode, provinceCodes));

        var baDinh = Assert.Single(communes, x => x.code == "00004");
        Assert.Equal("Phường Ba Đình", baDinh.name);
        Assert.Equal("Phường", baDinh.administrativeLevel);
        Assert.Equal("01", baDinh.provinceCode);
    }
}
