using Xunit;

namespace ConvertDanhSachTinhThanh.Tests;

public class ModelTests
{
    private static readonly Guid SampleId = Guid.Parse("8f14e45f-ceea-4672-9e1e-1f1d6c8b0a01");

    [Fact]
    public void DiaChiCu_Constructor_AssignsAllFields_AndLeavesSoNhaUnset()
    {
        var diaChi = new DiaChiCu(SampleId, "12 Lê Lợi", "Hồ Chí Minh", "Quận 1", "Bến Nghé");

        Assert.Equal(SampleId, diaChi.HoSo);
        Assert.Equal("12 Lê Lợi", diaChi.FullDiaChi);
        Assert.Equal("Hồ Chí Minh", diaChi.TenTinhThanhCu);
        Assert.Equal("Quận 1", diaChi.TenQuanHuyenCu);
        Assert.Equal("Bến Nghé", diaChi.TenXaPhuongCu);
        Assert.Null(diaChi.SoNha);
    }

    [Fact]
    public void DiaChiMoi_Constructor_AssignsHoSoAndAddress()
    {
        var diaChi = new DiaChiMoi(SampleId, "Phường Sài Gòn, Thành phố Hồ Chí Minh");

        Assert.Equal(SampleId, diaChi.HoSo);
        Assert.Equal("Phường Sài Gòn, Thành phố Hồ Chí Minh", diaChi.FullDiaChi);
        Assert.Null(diaChi.TinhThanhNew);
        Assert.Null(diaChi.XaPhuong_New);
    }

    [Fact]
    public void TinhThanhNew_ToString_ReturnsName()
    {
        var tinh = new TinhThanh_New(SampleId, "79", "Thành phố Hồ Chí Minh");

        Assert.Equal("79", tinh.MaQuanLy);
        Assert.Equal("Thành phố Hồ Chí Minh", tinh.ToString());
    }

    [Fact]
    public void XaPhuongNew_ToString_ReturnsName_AndKeepsProvinceLink()
    {
        var provinceId = Guid.NewGuid();
        var xa = new XaPhuong_New(SampleId, "26734", "Phường Sài Gòn", provinceId);

        Assert.Equal("26734", xa.MaQuanLy);
        Assert.Equal(provinceId, xa.TinhThanh_New);
        Assert.Equal("Phường Sài Gòn", xa.ToString());
    }

    [Fact]
    public void TinhThanhTrenCoSoDuLieu_And_QuanHuyenTrenCoSoDuLieu_AssignConstructorValues()
    {
        var tinh = new TinhThanhTrenCoSoDuLieu(SampleId, "01", "Hà Nội");
        var quan = new QuanHuyenTrenCoSoDuLieu(Guid.Empty, "001", "Ba Đình") { TinhThanh = tinh };

        Assert.Equal(("01", "Hà Nội"), (tinh.MaQuanLy, tinh.TenTinhThanh));
        Assert.Equal(("001", "Ba Đình"), (quan.MaQuanLy, quan.TenQuanHuyen));
        Assert.Same(tinh, quan.TinhThanh);
    }
}
