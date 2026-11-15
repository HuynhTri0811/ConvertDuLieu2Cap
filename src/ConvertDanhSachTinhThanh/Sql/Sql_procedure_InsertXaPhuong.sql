IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spd_InsertXaPhuongMoi')
BEGIN
    DROP PROCEDURE spd_InsertXaPhuongMoi;
END
GO

CREATE PROCEDURE spd_InsertXaPhuongMoi 
	@QueryText NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION


		DECLARE @QuocGia UNIQUEIDENTIFIER = (SELECT TOP 1 Oid FROM QuocGia WHERE TenQuocGia LIKE N'Việt Nam' OR MaQuanLy LIKE N'VN' AND GCRecord IS NULL)

		DECLARE @TABLE TABLE
		(
			MaQuanLy NVARCHAR(200),
			TenXaPhuong NVARCHAR(200),
			MaQuanLyTinhThanh NVARCHAR(200)
		)

		INSERT INTO @TABLE
		(
			MaQuanLy ,
			TenXaPhuong,
			MaQuanLyTinhThanh
		)
		EXEC(@QueryText)

		INSERT INTO XaPhuong_New
		(
			Oid,
			TinhThanh_New,
			MaQuanLy,
			TenXaPhuong
		)
		SELECT NEWID(),(SELECT TOP 1 Oid FROM TinhThanh_New x WHERE x.MaQuanLy = MaQuanLyTinhThanh),MaQuanLy,TenXaPhuong FROM @TABLE
		WHERE MaQuanLy NOT IN (SELECT MaQuanLy FROM XaPhuong_New)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END