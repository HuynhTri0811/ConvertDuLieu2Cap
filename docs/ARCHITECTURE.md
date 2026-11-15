# Kiến trúc và luồng xử lý

## Tổng quan

`ConvertDanhSachTinhThanh` là ứng dụng Windows Forms một màn hình, viết cho **.NET Framework 4.8**. Ứng dụng có ba nguồn dữ liệu:

- **XML danh mục** đi kèm ứng dụng (`Data/`), gồm danh mục cũ để đối chiếu và danh mục mới để nạp vào cơ sở dữ liệu.
- **SQL Server**, nơi chứa hồ sơ, địa chỉ cũ và các bảng danh mục/địa chỉ mới. Chuỗi kết nối được nhập trên giao diện.
- **Hai API chuyển đổi địa chỉ** bên ngoài (endpoint khai báo trong `AppConstants`).

```text
           XML danh mục cũ ──┐
                             ▼
SQL Server (HoSo, DiaChi) ─► Form1 ──► API chuyển đổi ──► Form1 ──► SQL Server (DiaChi_New, HoSo.*_New)
                             ▲                               ▲
           XML danh mục mới ─┴─► SQL Server (TinhThanh_New, XaPhuong_New)
```

## Thành phần

| Thư mục / file | Vai trò |
| --- | --- |
| `Program.cs` | Điểm vào, khởi chạy `Form1`. |
| `Forms/Form1.cs` | State dùng chung của form; constructor nạp ba danh mục cũ từ XML và điền ComboBox loại địa chỉ. |
| `Forms/Form1.Database.cs` | Các thao tác với SQL Server: nạp địa chỉ cũ, ánh xạ tên sang mã hành chính cũ, nạp danh mục mới, chạy script SQL và stored procedure. |
| `Forms/Form1.AddressConversion.cs` | Phân loại địa chỉ, gọi hai API chuyển đổi, tra cứu tỉnh/xã mới theo kết quả trả về. |
| `Forms/Form1.Designer.cs`, `Form1.resx` | Giao diện do Designer sinh ra; nối event handler. |
| `Forms/AutoWaitForm.cs` | Form chờ (hiện chưa có logic). |
| `Helpers/AddressQueryBuilder.cs` | Dựng câu SELECT địa chỉ cũ theo loại địa chỉ được chọn. |
| `Helpers/StringNormalizer.cs` | Bỏ tiền tố hành chính, bỏ dấu tiếng Việt, chuẩn hóa chuỗi để so sánh. |
| `Constants/AppConstants.cs` | Loại địa chỉ, câu truy vấn danh mục mới, endpoint API, thông báo, batch size. |
| `DataAccess/DatabaseHelper.cs` | Hàm dùng chung cho query, non-query và stored procedure. `Form1` hiện chưa dùng lớp này mà vẫn tự mở `SqlConnection`. |
| `Model/` | Model ánh xạ XML danh mục, model địa chỉ cũ/mới và DTO request/response của API. |

`Form1` là lớp `partial`. Các file trong `Forms/` được biên dịch thành cùng một lớp, nên control, state và event handler dùng chung với nhau.

`ConvertDanhSachTinhThanh.csproj` là project kiểu cũ và không tự động thêm file. Khi thêm file `.cs`, `.xml` hay `.sql`, cần khai báo thêm `<Compile Include>` hoặc `<Content Include>` tương ứng. File XML và SQL đặt `CopyToOutputDirectory=Always` để có mặt trong `bin/<Config>/Data/...` và `bin/<Config>/Sql/...`. Code đọc chúng qua `Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ...)`.

## Các nút trên giao diện

| Nút (text hiển thị) | Handler | File |
| --- | --- | --- |
| Kiểm tra kết nối | Chưa nối handler | — |
| Convert Tỉnh Thành | `button2_Click` | `Form1.Database.cs` |
| Thêm mới danh sách tỉnh thành mới vào cơ sở dữ liệu | `button3_Click` | `Form1.Database.cs` |
| Thêm mới danh sách xã phương mới vào cơ sở dữ liệu | `button6_Click` | `Form1.Database.cs` |
| Load danh sách địa chỉ cũ | `LoadDanhSachDiaChiCuTuTrongCoSoDuLieu_Click` | `Form1.Database.cs` |
| ConverDiaChiCuThanhDiaChiMoi | `button4_Click` | `Form1.AddressConversion.cs` |
| Test | `button7_Click` | `Form1.AddressConversion.cs` |
| Insert vào địa chỉ mới | `button5_Click` | `Form1.Database.cs` |

**Convert Tỉnh Thành** chỉ đọc bảng `TinhThanh` cũ trong cơ sở dữ liệu vào `listTinhThanh_CoTrenCoSoDuLieu`. Danh sách này hiện chưa được bước nào khác sử dụng.

## Quy trình sử dụng

Thứ tự thao tác thông thường:

### Bước 0: Khởi động

Constructor của `Form1` đọc ba file `Data/DanhSachCu/*.xml` bằng `XmlSerializer` (xem [DATA_AND_SQL.md](DATA_AND_SQL.md)). ComboBox được điền bốn loại địa chỉ từ `AppConstants`: *Địa chỉ thường trú*, *Nơi ở hiện nay*, *Nơi sinh*, *Quê quán*.

Nếu thiếu một file XML, ứng dụng sẽ lỗi ngay khi mở.

### Bước 1: Nạp danh mục mới vào cơ sở dữ liệu (làm một lần)

1. **Thêm mới danh sách tỉnh thành mới**: đọc `DanhSachTinhThanhMoi.xml`, chạy `Sql/Sql_procedure_InsertTinhThanh.sql` (tách theo `GO`) để tạo `spd_InsertTinhThanhMoi`. Sau đó gọi procedure với một chuỗi `SELECT ... UNION ALL ...` chứa mã và tên tỉnh.
2. **Thêm mới danh sách xã phường mới**: tương tự, với `DanhSachQuanHuyenMoi.xml` và `spd_InsertXaPhuongMoi`.

Cả hai procedure chỉ chèn những mã chưa có, nên chạy lại không tạo bản ghi trùng.

### Bước 2: Nạp địa chỉ cũ

**Load danh sách địa chỉ cũ** (`LoadDanhSachDiaChiCuTuTrongCoSoDuLieu_Click`):

1. Kiểm tra đã chọn loại địa chỉ và đã nhập chuỗi kết nối.
2. `AddressQueryBuilder.Build(loại)` trả về câu SELECT nối `HoSo` với `DiaChi` theo đúng cột (`DiaChiThuongTru`, `NoiOHienNay`, `NoiSinh`, `QueQuan`). Loại không hợp lệ trả về `null`, khi đó handler báo lỗi.
3. Với mỗi dòng, ánh xạ tên tỉnh, huyện, xã cũ sang **mã** trong danh mục cũ:
   - tỉnh: so khớp tên đã chuẩn hóa với `DanhSachTinhThanhCu.xml`;
   - huyện: so khớp tên trong cùng tỉnh;
   - xã: so khớp tên và kiểm tra `MaTenDonViCha` có chứa mã huyện.
   Trước khi so sánh, chuỗi được chuẩn hóa bằng `StringNormalizer`.
4. Kết quả lưu vào `listDiaChiCu` và hiển thị ở `dataGridView1`.

### Bước 3: Chuyển đổi

**ConverDiaChiCuThanhDiaChiMoi** (`button4_Click`):

1. Nạp `TinhThanh_New` và `XaPhuong_New` từ cơ sở dữ liệu.
2. Chia `listDiaChiCu` thành hai nhóm và xử lý song song:
   - **Đủ 3 cấp** (có tỉnh, huyện, xã): gửi `RequesModel_3Cap` (mã tỉnh/huyện/xã + số nhà) đến `API_ENDPOINT_CONVERT_3CAP` với Bearer token, nhận về `Root`.
   - **Thiếu cấp**: gửi `RequestModel` (chuỗi địa chỉ đầy đủ; `TP.` được thay bằng "Thành phố Hồ Chí Minh") đến `API_ENDPOINT_CONVERT_ADDRESS`, nhận về `ApiResponseDto`.
3. Từ mã hoặc tên trả về, tìm `TinhThanh_New` và `XaPhuong_New` tương ứng (có cache trong `listTinhThanhNew_TonTai` và `ListXaPhuongNew_TonTai`) rồi tạo `DiaChiMoi`.
4. Gom kết quả vào `listDiaChiMoi` và hiển thị ở `dataGridView2`.

Lỗi khi gọi API bị bỏ qua: bản ghi vẫn nằm trong danh sách nhưng không có tỉnh/xã mới, và sẽ bị bỏ qua ở bước 4.

**Test** (`button7_Click`) chỉ chạy luồng API 3 cấp cho toàn bộ `listDiaChiCu`.

### Bước 4: Ghi địa chỉ mới

**Insert vào địa chỉ mới** (`button5_Click`):

1. Chạy `Sql/Sql_procedure.sql` để tạo `spd_InsertDanhSachDiaChiMoi`.
2. Chia `listDiaChiMoi` thành từng lô 100 bản ghi, bỏ các bản ghi không có tỉnh mới hoặc không có địa chỉ đầy đủ.
3. Mỗi lô được dựng thành `SELECT ... UNION ALL ...` và truyền vào procedure cùng loại địa chỉ (`@Loai`).
4. Procedure tạo bản ghi `DiaChi_New`, cập nhật cột `*_New` tương ứng trên `HoSo` và sao chép số nhà từ địa chỉ cũ.

## Nguyên tắc khi phát triển tiếp

- Giữ `Form1` là lớp `partial` để tương thích với Designer và `.resx`. Đặt code mới vào file phù hợp với trách nhiệm.
- Logic thuần (không phụ thuộc WinForms/SQL) nên tách vào `Helpers/` hoặc `Model/`. Project test tự động biên dịch kèm các thư mục này nên có thể viết test ngay.
- Cập nhật `ConvertDanhSachTinhThanh.csproj` mỗi khi thêm file.
- Không đưa chuỗi kết nối hay khóa API vào tài liệu, test hoặc mã nguồn. Xem [KNOWN_ISSUES.md](KNOWN_ISSUES.md).
