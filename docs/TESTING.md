# Unit test

## Chạy test

Từ thư mục gốc repository:

```sh
dotnet test tests/ConvertDanhSachTinhThanh.Tests
```

Chỉ cần .NET SDK 10, chạy được trên Windows, macOS và Linux. Không cần SQL Server, mạng hay Windows Forms.

Chạy một nhóm test:

```sh
dotnet test tests/ConvertDanhSachTinhThanh.Tests --filter "FullyQualifiedName~CatalogXmlTests"
```

## Cách project test được tổ chức

Ứng dụng nhắm .NET Framework 4.8 và phụ thuộc WinForms, nên project test (.NET 10) không tham chiếu trực tiếp project ứng dụng. Thay vào đó, `ConvertDanhSachTinhThanh.Tests.csproj` **liên kết file nguồn** của các phần không phụ thuộc nền tảng:

| Nguồn liên kết | Hiển thị trong project test |
| --- | --- |
| `src/.../Constants/AppConstants.cs` | `App/Constants/` |
| `src/.../Helpers/*.cs` | `App/Helpers/` |
| `src/.../Model/*.cs` | `App/Model/` |
| `src/.../Data/**/*.xml` | `Data/` (copy ra output) |

`Newtonsoft.Json 13.0.3` được thêm vào, cùng phiên bản với ứng dụng, để test DTO đúng như lúc chạy thật.

File mới trong `Helpers/` hoặc `Model/` tự động được đưa vào project test nhờ wildcard. Không đặt vào hai thư mục này code phụ thuộc WinForms hoặc `System.Data.SqlClient`, vì project test sẽ không biên dịch được.

## Phạm vi kiểm thử

| File test | Kiểm tra |
| --- | --- |
| `StringNormalizerTests` | Bỏ tiền tố tỉnh/huyện/xã (tên bắt đầu bằng X như "Xuân La" được giữ nguyên); giữ nguyên `null` và chuỗi rỗng; bỏ dấu và ký tự đặc biệt; mở rộng `TP.`/`TP.HCM` đúng thành phố; hành vi khi truyền `null` cho hai hàm chuẩn hóa tổng quát. |
| `AddressQueryBuilderTests` | Mỗi loại địa chỉ nối đúng cột của `HoSo`; phần SELECT và điều kiện lọc dùng chung; các loại khác nhau cho câu truy vấn khác nhau; loại không hợp lệ (kể cả sai hoa/thường hoặc chuỗi dạng SQL) trả về `null`. |
| `CatalogXmlTests` | Đọc **file XML thật** bằng chính model và `XmlSerializer` như `Form1`; kiểm tra số bản ghi (63 / 707 / 10.599 / 34 / 3.321), bản ghi mẫu, mã không trùng, xã cũ trỏ tới mã huyện có thật, xã mới trỏ tới tỉnh mới có thật. |
| `ApiDtoTests` | JSON gửi đi của `RequestModel` và `RequesModel_3Cap`; đọc phản hồi của cả hai API (`ApiResponseDto`, `Root` với thuộc tính `new`); phản hồi lỗi không có `data`. |
| `ModelTests` | Constructor của các model địa chỉ/danh mục; `ToString()` của `TinhThanh_New` và `XaPhuong_New` (được dùng khi hiển thị trên lưới). |

## Chưa được kiểm thử

- Event handler trong `Forms/` (phụ thuộc WinForms, SQL Server và API thật).
- `DataAccess/DatabaseHelper` và các stored procedure trong `Sql/`.
- Logic đối chiếu tên → mã và tra cứu tỉnh/xã mới. Phần này đang nằm trong `Form1`; muốn test cần tách ra một lớp riêng trong `Helpers/`.

## Thêm test mới

1. Tạo file `*Tests.cs` trong `tests/ConvertDanhSachTinhThanh.Tests/`, namespace `ConvertDanhSachTinhThanh.Tests`, thêm `using Xunit;`.
2. Dùng `[Fact]` cho trường hợp đơn lẻ, `[Theory]` + `[InlineData]` cho nhiều bộ dữ liệu.
3. Nếu test file dữ liệu, đọc qua `AppContext.BaseDirectory` + `Data/...` như trong `CatalogXmlTests`.
4. Không đưa chuỗi kết nối, token hay dữ liệu cá nhân thật vào test.
