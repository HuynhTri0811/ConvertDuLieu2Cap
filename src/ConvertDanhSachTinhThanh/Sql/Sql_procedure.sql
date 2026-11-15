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


		
		DECLARE  @TinhThanhNew_Temp TABLE
		(
			HoSo_Oid UNIQUEIDENTIFIER,
			TinhThanh_New UNIQUEIDENTIFIER,
			XaPhuong_New UNIQUEIDENTIFIER,
			DiaChiFull NVARCHAR(MAX)
		)





		DECLARE @TABLE TABLE
		(
			HoSo_Oid UNIQUEIDENTIFIER,
			TinhThanh_New UNIQUEIDENTIFIER,
			XaPhuong_New UNIQUEIDENTIFIER,
			DiaChiFull NVARCHAR(MAX),
			DiaChi_New UNIQUEIDENTIFIER
		)

		INSERT INTO @TinhThanhNew_Temp
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
		SELECT HoSo_Oid,TinhThanh_New,XaPhuong_New,DiaChiFull,NEWID() FROM @TinhThanhNew_Temp


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


			UPDATE dcn
			SET SoNha = (SELECT TOP 1 dc.SoNha FROM DiaChi dc WHERE dc.Oid = hs.DiaChiThuongTru) 
			FROM HoSo hs
				JOIN DiaChi_New dcn ON hs.DiaChiThuongTru_New = dcn.Oid


		END
		IF @Loai = N'Nơi ở hiện nay'
		BEGIN
			UPDATE HoSo
			SET NoiOHienNay_New = x.DiaChi_New
			FROM HoSo hs
				JOIN @TABLE x ON x.HoSo_Oid = hs.Oid
			WHERE hs.GCRecord IS NULL

			UPDATE dcn
			SET SoNha = (SELECT TOP 1 dc.SoNha FROM DiaChi dc WHERE dc.Oid = hs.NoiOHienNay) 
			FROM HoSo hs
				JOIN DiaChi_New dcn ON hs.NoiOHienNay_New = dcn.Oid

		END

		IF @Loai = N'Nơi sinh'
		BEGIN
			UPDATE HoSo
			SET NoiSinh_New = x.DiaChi_New
			FROM HoSo hs
				JOIN @TABLE x ON x.HoSo_Oid = hs.Oid
			WHERE hs.GCRecord IS NULL

			UPDATE dcn
			SET SoNha = (SELECT TOP 1 dc.SoNha FROM DiaChi dc WHERE dc.Oid = hs.NoiSinh) 
			FROM HoSo hs
				JOIN DiaChi_New dcn ON hs.NoiSinh_New = dcn.Oid

		END

		IF @Loai = N'Quê quán'
		BEGIN
			UPDATE HoSo
			SET QueQuan_New = x.DiaChi_New
			FROM HoSo hs
				JOIN @TABLE x ON x.HoSo_Oid = hs.Oid
			WHERE hs.GCRecord IS NULL


			UPDATE dcn
			SET SoNha = (SELECT TOP 1 dc.SoNha FROM DiaChi dc WHERE dc.Oid = hs.QueQuan) 
			FROM HoSo hs
				JOIN DiaChi_New dcn ON hs.QueQuan_New = dcn.Oid
		END

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END