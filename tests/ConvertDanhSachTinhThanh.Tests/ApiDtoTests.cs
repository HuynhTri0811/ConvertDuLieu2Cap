using Newtonsoft.Json;
using Xunit;

namespace ConvertDanhSachTinhThanh.Tests;

/// <summary>
/// The app talks to both conversion APIs with Newtonsoft.Json; these tests pin
/// the JSON shapes the DTOs send and accept.
/// </summary>
public class ApiDtoTests
{
    [Fact]
    public void RequestModel_SerializesOldAddressField()
    {
        string json = JsonConvert.SerializeObject(new RequestModel { oldAddress = "12 Lê Lợi, Bến Nghé" });

        Assert.Equal("{\"oldAddress\":\"12 Lê Lợi, Bến Nghé\"}", json);
    }

    [Fact]
    public void RequestModel3Cap_SerializesAllFourFields()
    {
        var model = new RequesModel_3Cap
        {
            provinceCode = "79",
            districtCode = "760",
            wardCode = "26740",
            streetAddress = "12 Lê Lợi",
        };

        string json = JsonConvert.SerializeObject(model);

        Assert.Equal(
            "{\"provinceCode\":\"79\",\"districtCode\":\"760\",\"wardCode\":\"26740\",\"streetAddress\":\"12 Lê Lợi\"}",
            json);
    }

    [Fact]
    public void ApiResponseDto_DeserializesFullAddressResponse()
    {
        const string json = """
            {
              "newAddress": {
                "code": "26734",
                "commune": "Phường Sài Gòn",
                "province": "Thành phố Hồ Chí Minh",
                "fullAddress": "12 Lê Lợi, Phường Sài Gòn, Thành phố Hồ Chí Minh"
              }
            }
            """;

        var result = JsonConvert.DeserializeObject<ApiResponseDto>(json)!;

        Assert.Equal("26734", result.NewAddress.Code);
        Assert.Equal("Phường Sài Gòn", result.NewAddress.Commune);
        Assert.Equal("Thành phố Hồ Chí Minh", result.NewAddress.Province);
        Assert.Equal("12 Lê Lợi, Phường Sài Gòn, Thành phố Hồ Chí Minh", result.NewAddress.FullAddress);
    }

    [Fact]
    public void Root_Deserializes3CapResponse_IncludingReservedNewProperty()
    {
        const string json = """
            {
              "success": true,
              "message": "OK",
              "data": {
                "old": {
                  "province": { "code": "79", "name": "Thành phố Hồ Chí Minh", "type": "Thành phố" },
                  "district": { "code": "760", "name": "Quận 1", "type": "Quận" },
                  "ward": { "code": "26740", "name": "Phường Bến Nghé", "type": "Phường" },
                  "fullAddress": "12 Lê Lợi, Phường Bến Nghé, Quận 1, Thành phố Hồ Chí Minh"
                },
                "new": {
                  "province": { "code": "79", "name": "Thành phố Hồ Chí Minh", "type": "Thành phố" },
                  "ward": { "code": "26734", "name": "Phường Sài Gòn", "type": "Phường" },
                  "fullAddress": "12 Lê Lợi, Phường Sài Gòn, Thành phố Hồ Chí Minh"
                },
                "mergeInfo": { "notes": "Sáp nhập" }
              }
            }
            """;

        var result = JsonConvert.DeserializeObject<Root>(json)!;

        Assert.True(result.success);
        Assert.Equal("760", result.data.old.district.code);
        Assert.Equal("26740", result.data.old.ward.code);
        Assert.Equal("79", result.data.@new.province.code);
        Assert.Equal("26734", result.data.@new.ward.code);
        Assert.Equal("12 Lê Lợi, Phường Sài Gòn, Thành phố Hồ Chí Minh", result.data.@new.fullAddress);
        Assert.Equal("Sáp nhập", result.data.mergeInfo.notes);
    }

    [Fact]
    public void Root_FailedResponse_HasNullData()
    {
        var result = JsonConvert.DeserializeObject<Root>("{\"success\":false,\"message\":\"Not found\"}")!;

        Assert.False(result.success);
        Assert.Equal("Not found", result.message);
        Assert.Null(result.data);
    }
}
