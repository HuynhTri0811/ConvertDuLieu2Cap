# Tài liệu cấu trúc và refactor

## Tổng quan

Ứng dụng chuyển đổi danh mục và địa chỉ hành chính cũ sang danh mục mới. Giao diện được xây bằng Windows Forms, đọc dữ liệu danh mục từ XML, truy xuất hồ sơ từ SQL Server và gửi địa chỉ đến API chuyển đổi.

Ứng dụng chính nhắm đến **.NET Framework 4.8**. `Form1` được chia thành các file `partial` theo trách nhiệm; khi biên dịch, các file này vẫn hợp thành cùng một lớp `Form1`. Vì vậy các control do Designer tạo, state dùng chung và event đã nối trong giao diện tiếp tục hoạt động như trước.

## Cấu trúc file

| File | Vai trò |
| --- | --- |
| `ConvertDanhSachTinhThanh/Form1.cs` | Khai báo state chung, khởi tạo form, nạp các danh mục hành chính cũ từ XML và thêm lựa chọn loại địa chỉ vào ComboBox. |
| `ConvertDanhSachTinhThanh/Form1.Database.cs` | Xử lý truy vấn SQL, nạp địa chỉ cũ, ánh xạ tên/mã hành chính, nạp danh mục mới và ghi kết quả vào cơ sở dữ liệu. |
| `ConvertDanhSachTinhThanh/Form1.AddressConversion.cs` | Phân loại địa chỉ, gọi API chuyển đổi, tra cứu tỉnh/phường mới và ghép kết quả để hiển thị. |
| `ConvertDanhSachTinhThanh/Form1.Designer.cs` | Mã giao diện được Windows Forms Designer sinh ra; khai báo control và gắn event handler. |
| `ConvertDanhSachTinhThanh/Helpers/AddressQueryBuilder.cs` | Dựng câu SELECT theo loại địa chỉ được chọn. |
| `ConvertDanhSachTinhThanh/Helpers/StringNormalizer.cs` | Chuẩn hóa tên hành chính và chuỗi tiếng Việt. |
| `ConvertDanhSachTinhThanh/Constants/AppConstants.cs` | Hằng số cho loại địa chỉ, một số truy vấn SQL, endpoint, thông báo và batch size. |
| `ConvertDanhSachTinhThanh/DataAccess/DatabaseHelper.cs` | Các hàm dùng chung để chạy query, non-query và stored procedure. Các event handler hiện tại trong `Form1` vẫn có một số đoạn thao tác SQL trực tiếp. |
| `ConvertDanhSachTinhThanh/Model/` | Model dữ liệu địa chỉ, danh mục tỉnh/phường và DTO gửi/nhận API. |
| `ConvertDanhSachTinhThanh.Tests/` | Unit test hiện có cho `StringNormalizer`. |

`Form1.Database.cs` và `Form1.AddressConversion.cs` được khai báo tường minh trong file project cũ `ConvertDanhSachTinhThanh.csproj`. Nếu thêm file `.cs` mới vào project này, cần thêm một mục `<Compile Include="..." />` tương ứng.

## Luồng xử lý trên giao diện

Các tên handler bên dưới được nối với control trong `Form1.Designer.cs`.

### 1. Khởi tạo danh mục cũ

Constructor của `Form1` nạp danh sách tỉnh, quận/huyện và xã/phường cũ từ thư mục `DanhSachCauTrucCu`. Các hàm nạp dùng `XmlSerializer`, sau đó chuyển `Items` thành danh sách để phục vụ đối chiếu trong lúc đọc địa chỉ.

Các lựa chọn trong ComboBox được lấy từ `AppConstants`:

- Địa chỉ thường trú
- Nơi ở hiện nay
- Nơi sinh
- Quê quán

### 2. Nạp địa chỉ cũ

Nút **Load danh sách địa chỉ cũ** gọi `LoadDanhSachDiaChiCuTuTrongCoSoDuLieu_Click` trong `Form1.Database.cs`:

1. Kiểm tra loại địa chỉ và chuỗi kết nối.
2. Gọi `AddressQueryBuilder.Build` để tạo câu truy vấn phù hợp với lựa chọn.
3. Đọc hồ sơ và địa chỉ từ SQL Server.
4. Đối chiếu tên tỉnh, quận/huyện, xã/phường với danh mục cũ; chuẩn hóa chuỗi trước khi so sánh.
5. Đưa kết quả vào `listDiaChiCu` và hiển thị trên `dataGridView1`.

Nếu loại địa chỉ không được hỗ trợ, `Build` trả về `null` và handler báo lỗi trước khi chạy truy vấn.

### 3. Chuyển đổi địa chỉ

Nút **ConverDiaChiCuThanhDiaChiMoi** gọi `button4_Click` trong `Form1.AddressConversion.cs`. Handler chia dữ liệu thành hai nhóm:

- Địa chỉ có đủ tỉnh, quận/huyện và xã/phường: gửi mã ba cấp đến API ba cấp.
- Địa chỉ thiếu một trong các cấp: gửi chuỗi địa chỉ đầy đủ đến API chuyển đổi địa chỉ.

Kết quả API được chuyển thành `DiaChiMoi`. Mã tỉnh và phường trả về được tìm trong danh mục mới; kết quả được gom vào `listDiaChiMoi` và hiển thị trên `dataGridView2`.

Nút **Test** (`button7_Click`) chạy riêng luồng gọi API ba cấp trên danh sách hiện tại. Nó được giữ theo event hiện có trong giao diện.

### 4. Ghi danh mục và địa chỉ mới

- Nút **Thêm mới danh sách tỉnh thành mới vào cơ sở dữ liệu** đọc `DanhSachTinhThanhMoi.xml`, chạy script SQL và gọi stored procedure chèn tỉnh thành.
- Nút **Thêm mới danh sách xã phương mới vào cơ sở dữ liệu** đọc `DanhSachQuanHuyenMoi.xml`, chạy script và gọi stored procedure tương ứng.
- Nút **Insert vào địa chỉ mới** chia `listDiaChiMoi` theo batch, tạo dữ liệu chèn và gọi stored procedure lưu địa chỉ.

## Helpers và model

### `AddressQueryBuilder`

`AddressQueryBuilder.Build(addressType)` giữ phần SELECT dùng chung ở một chỗ, còn phần JOIN được chọn theo loại địa chỉ. Điều này loại bỏ bốn khối SQL giống nhau trong event handler. Việc ánh xạ lựa chọn sang tên cột được kiểm soát trong code; giá trị ComboBox không được chèn trực tiếp vào SQL.

### `StringNormalizer`

Các hàm công khai hiện có:

- `NormalizeProvinceName`: bỏ một số tiền tố tỉnh/thành phố.
- `NormalizeDistrictName`: bỏ tiền tố quận, huyện, thị xã, thành phố và các dạng viết tắt hiện được hỗ trợ.
- `NormalizeWardName`: bỏ tiền tố phường, xã, thị trấn và một số dạng viết tắt.
- `StandardizeString`: chuyển chữ thường, tách dấu Unicode, bỏ dấu và loại ký tự ngoài chữ/số ASCII.
- `ConvertVietnameseAbbreviations`: thay chuỗi `TP.` bằng cụm từ tiếng Việt đang được cấu hình trong hàm.

Các hàm `Normalize...` giữ nguyên `null` và chuỗi rỗng. Hai hàm chuẩn hóa tổng quát hiện chưa có xử lý riêng cho `null`; unit test ghi nhận hành vi hiện tại này.

### Model địa chỉ

- `DiaChiCu` chứa mã hồ sơ, địa chỉ đầy đủ, các cấp hành chính cũ và số nhà.
- `DiaChiMoi` chứa mã hồ sơ, tham chiếu tỉnh/phường mới và địa chỉ đầy đủ mới.
- Các request/response DTO trong `DiaChiCu.cs` ánh xạ dữ liệu trao đổi với hai API.

## Unit test

Test project dùng xUnit, nhắm đến .NET 10 và liên kết trực tiếp `StringNormalizer.cs`. Cách này cho phép kiểm thử helper độc lập mà không cần nạp WinForms, XML danh mục hay SQL Server.

Chạy test từ thư mục gốc repository:

```sh
dotnet test ConvertDanhSachTinhThanh.Tests/ConvertDanhSachTinhThanh.Tests.csproj
```

Test hiện bao phủ:

- Các tiền tố tỉnh/thành phố, quận/huyện và xã/phường.
- `null` và chuỗi rỗng ở các hàm `Normalize...`.
- Bỏ dấu tiếng Việt, đổi chữ thường và loại dấu câu/ký tự đặc biệt.
- Thay `TP.` theo đúng kiểu chữ hiện được hỗ trợ.
- Hành vi hiện tại khi truyền `null` cho `StandardizeString` và `ConvertVietnameseAbbreviations`.

Các thao tác SQL, event WinForms và gọi API chưa có unit test trong test project này.

## Build và môi trường

Để build ứng dụng WinForms, môi trường Windows cần có .NET Framework 4.8 Developer Pack/reference assemblies và các dependency được khai báo trong project.

Môi trường macOS hiện tại không có reference assemblies của .NET Framework 4.8; vì vậy lệnh `dotnet build ConvertDanhSachTinhThanh/ConvertDanhSachTinhThanh.csproj` dừng ở lỗi `MSB3644`. Lỗi này xảy ra trước khi compiler kiểm tra đầy đủ mã nguồn ứng dụng. Test project .NET 10 là project riêng và không thay thế việc build ứng dụng WinForms trên Windows.

## Nguyên tắc khi tiếp tục refactor

- Giữ nguyên `Form1` là lớp `partial` để tương thích với `Form1.Designer.cs` và `Form1.resx`.
- Đặt xử lý giao diện trong file phù hợp với trách nhiệm; logic thuần có thể kiểm thử nên tiếp tục được tách thành helper.
- Khi thêm file source, cập nhật `ConvertDanhSachTinhThanh.csproj` vì đây là project kiểu cũ, không tự động glob toàn bộ file `.cs`.
- Không ghi thông tin kết nối hoặc bí mật API vào tài liệu hay test fixture.
