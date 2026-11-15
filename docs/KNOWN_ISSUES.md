# Vấn đề đã biết

Danh sách được ghi lại khi rà soát mã nguồn. Các mục này **chưa được sửa** trong code; tài liệu chỉ mô tả hiện trạng và hướng xử lý đề xuất. Sắp xếp theo mức độ ưu tiên.

## Bảo mật

### 1. Token API nằm trong mã nguồn

`AppConstants.API_AUTH_TOKEN` chứa Bearer token của API chuyển đổi 3 cấp và đã có trong lịch sử git.

**Đề xuất:** thu hồi token hiện tại với nhà cung cấp, cấp token mới, rồi đọc token từ `App.config` (không commit), biến môi trường hoặc Windows Credential Manager.

### 2. SQL dựng bằng cách nối chuỗi

`button3_Click`, `button6_Click` và `button5_Click` ghép tên, mã và địa chỉ đầy đủ vào chuỗi `SELECT N'...' UNION ALL ...`. Chuỗi này sau đó được chạy bằng `EXEC(...)` trong stored procedure. Giá trị chứa dấu nháy đơn (`'`) sẽ làm hỏng câu lệnh. Riêng `FullDiaChi` lấy từ API bên ngoài nên còn là điểm có thể bị SQL injection.

**Đề xuất:** truyền dữ liệu bằng table-valued parameter hoặc `SqlBulkCopy` vào bảng tạm, thay cho chuỗi SQL động.

## Tính đúng đắn

### 3. Lỗi API bị bỏ qua

`ConvertAddressAsync` và `ConvertAddressAsync_3Cap` bắt mọi exception rồi `return`. Bản ghi vẫn có trong lưới nhưng không có tỉnh/xã mới, và người dùng không biết bản ghi nào bị lỗi hay vì sao.

**Đề xuất:** lưu thông báo lỗi vào `DiaChiMoi` (ví dụ thêm thuộc tính `LoiChuyenDoi`) và hiển thị trên lưới.

### 4. Lô rỗng khi số địa chỉ chia hết cho 100

`button5_Click` chạy vòng lặp `SoTongTrang + 1` lần. Khi số bản ghi chia hết cho 100, hoặc khi cả lô đều bị lọc bỏ, `@SqlInsert` là chuỗi rỗng và `EXEC('')` không chèn gì. Hiện điều này không gây lỗi, nhưng tạo thêm một lần gọi thừa. Ngoài ra, batch size đang viết cứng là `100`, không dùng `AppConstants.BATCH_SIZE`.

### 5. Tên huyện cha lệch trong `XaPhuongCu.xml`

Khoảng 590 xã thuộc các thị xã ghi `MaTenDonViCha` dạng `407-ThịNghiSơn-ThanhHóa`, trong khi `DanhSachQuanHuyenCu.xml` ghi `407-NghiSơn-ThanhHóa`. Ứng dụng so khớp theo **mã** huyện nên chưa bị ảnh hưởng. Tuy vậy, mọi logic sau này so khớp theo tên sẽ bị sai.

### 6. Khớp mã huyện bằng `Contains`

`TimRaDonViChaThongQuaMa` dùng `maTenDonViCha.Contains(maHuyen)`. Mã ngắn có thể khớp nhầm với phần tên hoặc mã khác trong chuỗi.

**Đề xuất:** so sánh chính xác phần mã đứng trước dấu `-` đầu tiên.

### 7. Thuộc tính `englistName` gõ sai

`Model/QuanHuyen.cs` khai báo `englistName`, trong khi XML dùng `<englishName>`, nên giá trị này luôn `null`. Hiện chưa có chỗ nào dùng trường này.

## Giao diện và mã nguồn

- Nút **Kiểm tra kết nối** chưa được nối handler.
- `AutoWaitForm` là lớp rỗng.
- `DatabaseHelper` đã có nhưng `Form1` chưa dùng; mỗi handler tự lặp lại phần mở kết nối và đọc dữ liệu.
- Một số tên có lỗi chính tả nhưng đang được dùng rộng: `LisstTinhThanh`, `RequesModel_3Cap`, `Connvectstring`, nút `ConverDiaChiCuThanhDiaChiMoi`. Đổi tên cần làm cùng lúc với Designer.
- `View_DiaChi.sql` chứa phiên bản cũ của `spd_InsertDanhSachDiaChiMoi`, không phải view. Nên đổi tên hoặc xóa.
- Lỗi được báo bằng `throw new Exception(...)` bên trong event handler. Trên WinForms, cách này hiện hộp thoại lỗi chưa xử lý thay vì một `MessageBox` thân thiện.
