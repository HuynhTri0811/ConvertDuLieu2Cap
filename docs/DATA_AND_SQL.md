# Dữ liệu danh mục và SQL

## File danh mục XML

Tất cả nằm trong `src/ConvertDanhSachTinhThanh/Data/` và được copy sang thư mục output với cùng cấu trúc.

### Danh mục cũ (3 cấp): `Data/DanhSachCu/`

Dùng để đối chiếu tên trong địa chỉ cũ sang mã hành chính cũ. Tên được lưu **không có khoảng trắng** (ví dụ `BaĐình`, `HàNội`).

| File | Phần tử gốc / bản ghi | Model | Số bản ghi | Trường |
| --- | --- | --- | --- | --- |
| `DanhSachTinhThanhCu.xml` | `<root>` / `<data>` | `LisstTinhThanhCu` → `TinhThanhCu` | 63 | `province_id`, `code`, `name`, `type` |
| `DanhSachQuanHuyenCu.xml` | `<DanhSachDonVi>` / `<DonVi>` | `ListQuanHuyenCu` → `QuanHuyenCu` | 707 | `Ma`, `Ten`, `TenDonViCha`, `MaTen`, `MaTenDonViCha` |
| `XaPhuongCu.xml` | `<DanhSachDonVi>` / `<DonVi>` | `ListXaPhuongCu` → `XaPhuongCu` | 10.599 | như trên |

`MaTen` có dạng `<mã>-<tên>-<tên đơn vị cha>`, ví dụ `001-BaĐình-HàNội`. `MaTenDonViCha` trỏ tới `MaTen` của đơn vị cha. Ứng dụng tìm xã thuộc huyện bằng cách kiểm tra `MaTenDonViCha` có chứa **mã** huyện hay không.

### Danh mục mới (2 cấp): `Data/DanhSachMoi/`

Dùng để nạp vào bảng `TinhThanh_New` và `XaPhuong_New`.

| File | Phần tử gốc / bản ghi | Model | Số bản ghi | Trường |
| --- | --- | --- | --- | --- |
| `DanhSachTinhThanhMoi.xml` | `<root>` / `<provinces>` | `LisstTinhThanh` → `TinhThanh` | 34 | `code`, `name`, `englishName`, `administrativeLevel`, `decree` |
| `DanhSachQuanHuyenMoi.xml` | `<root>` / `<communes>` | `ListQuanHuyen` → `QuanHuyen` | 3.321 | `code`, `name`, `englishName`, `administrativeLevel`, `provinceCode`, `provinceName`, `decree` |

Dù tên file là `DanhSachQuanHuyenMoi.xml`, nội dung là danh sách **xã/phường** mới. Mỗi bản ghi liên kết với tỉnh qua `provinceCode`.

Số bản ghi và các liên kết cha–con được kiểm tra tự động trong `CatalogXmlTests` (xem [TESTING.md](TESTING.md)). Khi cập nhật file XML, cần chạy lại test và sửa các con số kỳ vọng nếu dữ liệu thay đổi có chủ đích.

## Script SQL: `Sql/`

Mỗi script xóa procedure cũ (nếu có), sau `GO` tạo lại procedure. Ứng dụng tách script theo dòng `GO` và chạy từng phần trước khi gọi procedure, nên **không cần cài procedure thủ công**.

| Script | Procedure | Tham số | Được gọi từ |
| --- | --- | --- | --- |
| `Sql_procedure_InsertTinhThanh.sql` | `spd_InsertTinhThanhMoi` | `@QueryText NVARCHAR(MAX)` | `button3_Click` |
| `Sql_procedure_InsertXaPhuong.sql` | `spd_InsertXaPhuongMoi` | `@QueryText NVARCHAR(MAX)` | `button6_Click` |
| `Sql_procedure.sql` | `spd_InsertDanhSachDiaChiMoi` | `@SqlInsert NVARCHAR(MAX)`, `@Loai NVARCHAR(200)` | `button5_Click` |
| `View_DiaChi.sql` | `spd_InsertDanhSachDiaChiMoi` (bản cũ) | như trên | Không dùng |

Cách hoạt động chung: tham số chứa một câu `SELECT ... UNION ALL ...` do ứng dụng dựng sẵn. Procedure chạy câu này bằng `INSERT INTO @TABLE EXEC(@...)`, sau đó chèn hoặc cập nhật từ biến bảng, tất cả trong một transaction.

- **`spd_InsertTinhThanhMoi`**: chèn vào `TinhThanh_New` các tỉnh có `MaQuanLy` chưa tồn tại, gắn với quốc gia Việt Nam.
- **`spd_InsertXaPhuongMoi`**: chèn vào `XaPhuong_New` các xã chưa tồn tại, liên kết tỉnh qua `MaQuanLyTinhThanh` → `TinhThanh_New.MaQuanLy`.
- **`spd_InsertDanhSachDiaChiMoi`**: tạo `DiaChi_New` cho từng hồ sơ. Tùy `@Loai`, procedure cập nhật `HoSo.DiaChiThuongTru_New`, `NoiOHienNay_New`, `NoiSinh_New` hoặc `QueQuan_New`, rồi sao chép `SoNha` từ địa chỉ cũ.

`View_DiaChi.sql` không tạo view như tên gọi. Đây là một phiên bản cũ của `spd_InsertDanhSachDiaChiMoi` (dùng bảng tạm `TinhThanhNew_Temp`, chỉ xử lý hai loại địa chỉ). File được giữ lại để tham khảo và không được copy ra output.

## Bảng cơ sở dữ liệu được dùng

| Bảng | Đọc / ghi | Ghi chú |
| --- | --- | --- |
| `HoSo` | Đọc, cập nhật | Cột địa chỉ cũ: `DiaChiThuongTru`, `NoiOHienNay`, `NoiSinh`, `QueQuan`; cột mới tương ứng có hậu tố `_New`. Lọc `GCRecord IS NULL`. |
| `DiaChi` | Đọc | `FullDiaChi`, `SoNha`, `TinhThanh`, `QuanHuyen`, `XaPhuong`. |
| `TinhThanh`, `QuanHuyen`, `XaPhuong` | Đọc | Danh mục cũ trong cơ sở dữ liệu. |
| `QuocGia` | Đọc | Tìm Việt Nam theo `TenQuocGia` hoặc `MaQuanLy = 'VN'`. |
| `TinhThanh_New`, `XaPhuong_New` | Đọc, ghi | Danh mục mới. |
| `DiaChi_New` | Ghi | Địa chỉ sau chuyển đổi. |
