IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spd_InsertDanhSachDiaChiMoi')
BEGIN
    DROP PROCEDURE spd_InsertDanhSachDiaChiMoi;
END
GO
CREATE PROCEDURE spd_InsertDanhSachDiaChiMoi
@SqlInsert NVARCHAR(MAX),
@Loai NVARCHAR(200)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION

		IF EXISTS ( SELECT * FROM sys.tables WHERE name = 'TinhThanhNew_Temp')
		BEGIN
			DROP TABLE TinhThanhNew_Temp;
		END
		
		CREATE TABLE TinhThanhNew_Temp
		(
			HoSo_Oid UNIQUEIDENTIFIER,
			TinhThanh_New UNIQUEIDENTIFIER,
			XaPhuong_New UNIQUEIDENTIFIER,
			DiaChiFull NVARCHAR(200)
		)





		DECLARE @TABLE TABLE
		(
			HoSo_Oid UNIQUEIDENTIFIER,
			TinhThanh_New UNIQUEIDENTIFIER,
			XaPhuong_New UNIQUEIDENTIFIER,
			DiaChiFull NVARCHAR(200),
			DiaChi_New UNIQUEIDENTIFIER
		)

		INSERT INTO TinhThanhNew_Temp
		(
			HoSo_Oid ,
			TinhThanh_New,
			XaPhuong_New,
			DiaChiFull
		)
		EXEC(@SqlInsert)

		INSERT INTO @TABLE
		(
			HoSo_Oid,
			TinhThanh_New,
			XaPhuong_New,
			DiaChiFull,
			DiaChi_New
		)
		SELECT HoSo_Oid,TinhThanh_New,XaPhuong_New,DiaChiFull,NEWID() FROM TinhThanhNew_Temp


		INSERT INTO DiaChi_New
		(
			Oid,
			QuocGia,
			TinhThanh_New,
			XaPhuong_New,
			FullDiaChi
		)
		SELECT DiaChi_New,(SELECT TOP 1 Oid FROM QuocGia WHERE (TenQuocGia LIKE N'Việt Nam' OR MaQuanLy = N'VN') AND GCRecord IS NULL),TinhThanh_New,XaPhuong_New,DiaChiFull FROM @TABLE

		IF @Loai = N'Địa chỉ thường trú'
		BEGIN
			UPDATE HoSo
			SET DiaChiThuongTru_New = x.DiaChi_New
			FROM HoSo hs
				JOIN @TABLE x ON x.HoSo_Oid = hs.Oid
			WHERE hs.GCRecord IS NULL
		END
		IF @Loai = N'Nơi ở hiện nay'
		BEGIN
			UPDATE HoSo
			SET NoiOHienNay_New = x.DiaChi_New
			FROM HoSo hs
				JOIN @TABLE x ON x.HoSo_Oid = hs.Oid
			WHERE hs.GCRecord IS NULL
		END

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END