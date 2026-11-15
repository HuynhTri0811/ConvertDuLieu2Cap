using Xunit;
using ConvertDanhSachTinhThanh.Helpers;

namespace ConvertDanhSachTinhThanh.Tests;

public class StringNormalizerTests
{
    [Theory]
    [InlineData("Tỉnh An Giang", " An Giang")]
    [InlineData("Thành phố Hồ Chí Minh", " Hồ Chí Minh")]
    [InlineData("TP. Đà Nẵng", " Đà Nẵng")]
    [InlineData("tp Cần Thơ", " Cần Thơ")]
    [InlineData("Hà Nội", "Hà Nội")]
    public void NormalizeProvinceName_RemovesKnownPrefixes(string input, string expected)
    {
        Assert.Equal(expected, StringNormalizer.NormalizeProvinceName(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NormalizeProvinceName_PreservesNullOrEmpty(string? input)
    {
        Assert.Equal(input, StringNormalizer.NormalizeProvinceName(input));
    }

    [Theory]
    [InlineData("Quận 1", " 1")]
    [InlineData("Huyện Củ Chi", " Củ Chi")]
    [InlineData("Thành phố Thủ Đức", " Thủ Đức")]
    [InlineData("Thị xã Sơn Tây", " Sơn Tây")]
    [InlineData("TP Bến Tre", " Bến Tre")]
    [InlineData("TX Long Khánh", " Long Khánh")]
    [InlineData("TT Tràm Chim", " Tràm Chim")]
    public void NormalizeDistrictName_RemovesKnownPrefixes(string input, string expected)
    {
        Assert.Equal(expected, StringNormalizer.NormalizeDistrictName(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NormalizeDistrictName_PreservesNullOrEmpty(string? input)
    {
        Assert.Equal(input, StringNormalizer.NormalizeDistrictName(input));
    }

    [Theory]
    [InlineData("Phường Bến Nghé", " Bến Nghé")]
    [InlineData("Xã Tân An", " Tân An")]
    [InlineData("Thị trấn Củ Chi", " Củ Chi")]
    [InlineData("P. 1", " 1")]
    [InlineData("xã Bình Hưng", " Bình Hưng")]
    public void NormalizeWardName_RemovesKnownPrefixes(string input, string expected)
    {
        Assert.Equal(expected, StringNormalizer.NormalizeWardName(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NormalizeWardName_PreservesNullOrEmpty(string? input)
    {
        Assert.Equal(input, StringNormalizer.NormalizeWardName(input));
    }

    [Theory]
    [InlineData("Phường Xuân La", " Xuân La")]
    [InlineData("Xã Xuân Thu", " Xuân Thu")]
    [InlineData("X. Tân An", " Tân An")]
    [InlineData("Phường Xa La", " Xa La")]
    public void NormalizeWardName_KeepsNamesStartingWithX(string input, string expected)
    {
        Assert.Equal(expected, StringNormalizer.NormalizeWardName(input));
    }

    [Fact]
    public void NormalizeWardName_WardStartingWithX_MatchesOldCatalogName()
    {
        // Form1.GetDanhSachXaPhuongCu compares like this against XaPhuongCu.xml, where the name is "XuânLa".
        string fromDatabase = StringNormalizer.NormalizeWardName("Phường Xuân La").Replace(" ", "");

        Assert.Equal(StringNormalizer.StandardizeString("XuânLa"), StringNormalizer.StandardizeString(fromDatabase));
    }

    [Theory]
    [InlineData("Đường Lê Lợi, P. Bến Nghé", "duongleloipbennghe")]
    [InlineData("  Hà Nội  ", "hanoi")]
    [InlineData("Café #1", "cafe1")]
    [InlineData("", "")]
    public void StandardizeString_LowercasesRemovesDiacriticsAndPunctuation(string input, string expected)
    {
        Assert.Equal(expected, StringNormalizer.StandardizeString(input));
    }

    [Fact]
    public void StandardizeString_NullInput_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => StringNormalizer.StandardizeString(null!));
    }

    [Theory]
    [InlineData("TP. Hồ Chí Minh", "Thành phố Hồ Chí Minh")]
    [InlineData("TP. Hà Nội", "Thành phố Hà Nội")]
    [InlineData("12 Trần Phú, TP.Đà Nẵng", "12 Trần Phú, Thành phố Đà Nẵng")]
    [InlineData("TP. Thủ Đức, TP. Hồ Chí Minh", "Thành phố Thủ Đức, Thành phố Hồ Chí Minh")]
    [InlineData("12 Lê Lợi, Quận 1, TP.HCM", "12 Lê Lợi, Quận 1, Thành phố Hồ Chí Minh")]
    [InlineData("12 Lê Lợi, Q.1, Tp HCM", "12 Lê Lợi, Q.1, Thành phố Hồ Chí Minh")]
    [InlineData("12 Lê Lợi, Quận 1, TPHCM", "12 Lê Lợi, Quận 1, Thành phố Hồ Chí Minh")]
    [InlineData("12 Lê Lợi, Quận 1, TP.", "12 Lê Lợi, Quận 1, Thành phố Hồ Chí Minh")]
    [InlineData("tp. Hồ Chí Minh", "tp. Hồ Chí Minh")]
    [InlineData("Thôn TPhú, Xã An Bình", "Thôn TPhú, Xã An Bình")]
    [InlineData("Không có viết tắt", "Không có viết tắt")]
    public void ConvertVietnameseAbbreviations_ExpandsCityAbbreviations(string input, string expected)
    {
        Assert.Equal(expected, StringNormalizer.ConvertVietnameseAbbreviations(input));
    }

    [Fact]
    public void ConvertVietnameseAbbreviations_NullInput_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => StringNormalizer.ConvertVietnameseAbbreviations(null!));
    }
}
