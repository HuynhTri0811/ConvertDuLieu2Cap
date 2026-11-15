# ConvertDuLieu2Cap

Công cụ Windows Forms chuyển địa chỉ hành chính **3 cấp cũ** (tỉnh → quận/huyện → xã/phường) sang **2 cấp mới** (tỉnh → xã/phường) sau đợt sắp xếp đơn vị hành chính năm 2025.

Ứng dụng đọc hồ sơ từ SQL Server, đối chiếu địa chỉ với danh mục cũ, gọi API chuyển đổi rồi ghi địa chỉ mới trở lại cơ sở dữ liệu.

## Yêu cầu

| Mục đích | Cần có |
| --- | --- |
| Build và chạy ứng dụng | Windows, Visual Studio hoặc MSBuild, .NET Framework 4.8 Developer Pack |
| Chạy unit test | .NET SDK 10 (Windows, macOS hoặc Linux) |
| Sử dụng | Chuỗi kết nối SQL Server có các bảng `HoSo`, `DiaChi`, `TinhThanh`, `QuanHuyen`, `XaPhuong`, `QuocGia`, `TinhThanh_New`, `XaPhuong_New`, `DiaChi_New` |

## Bắt đầu nhanh

```sh
# Unit test (chạy được trên mọi hệ điều hành)
dotnet test tests/ConvertDanhSachTinhThanh.Tests

# Ứng dụng (Windows)
nuget restore ConvertDanhSachTinhThanh.sln
msbuild ConvertDanhSachTinhThanh.sln /p:Configuration=Release
```

Hoặc mở `ConvertDanhSachTinhThanh.sln` bằng Visual Studio rồi nhấn F5.

## Cấu trúc thư mục

```text
ConvertDanhSachTinhThanh.sln
src/ConvertDanhSachTinhThanh/         Ứng dụng WinForms (.NET Framework 4.8)
├── Forms/                            Form1 (chia thành các file partial), AutoWaitForm
├── Model/                            Model danh mục, địa chỉ và DTO của API
├── Helpers/                          StringNormalizer, AddressQueryBuilder
├── Constants/                        AppConstants
├── DataAccess/                       DatabaseHelper
├── Data/DanhSachCu/                  XML danh mục cũ (3 cấp)
├── Data/DanhSachMoi/                 XML danh mục mới (2 cấp)
├── Sql/                              Script tạo stored procedure
└── Properties/
tests/ConvertDanhSachTinhThanh.Tests/ Unit test xUnit (.NET 10)
docs/                                 Tài liệu
```

## Tài liệu

| Tài liệu | Nội dung |
| --- | --- |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Thành phần, các nút trên giao diện và luồng xử lý |
| [docs/DATA_AND_SQL.md](docs/DATA_AND_SQL.md) | File danh mục XML, stored procedure và bảng dữ liệu |
| [docs/TESTING.md](docs/TESTING.md) | Cách chạy test, phạm vi kiểm thử, cách thêm test |
| [docs/KNOWN_ISSUES.md](docs/KNOWN_ISSUES.md) | Các vấn đề đã biết và hướng khắc phục |
