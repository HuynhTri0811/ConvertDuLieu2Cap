IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spd_InsertTinhThanhMoi')
BEGIN
    DROP PROCEDURE spd_InsertTinhThanhMoi;
END
GO

CREATE PROCEDURE spd_InsertTinhThanhMoi 
	@QueryText NVARCHAR(MAX)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION


		DECLARE @QuocGia UNIQUEIDENTIFIER = (SELECT TOP 1 Oid FROM QuocGia WHERE TenQuocGia LIKE N'Việt Nam' OR MaQuanLy LIKE N'VN' AND GCRecord IS NULL)

		DECLARE @TABLE TABLE
		(
			MaQuanLy NVARCHAR(200),
			TenTinhThanh NVARCHAR(200)
		)

		INSERT INTO @TABLE
		(
			MaQuanLy ,
			TenTinhThanh
		)
		EXEC(@QueryText)

		INSERT INTO TinhThanh_New
		(
			Oid,
			QuocGia,
			MaQuanLy,
			TenTinhThanh
		)
		SELECT NEWID(),@QuocGia,MaQuanLy,TenTinhThanh FROM @TABLE
		WHERE MaQuanLy NOT IN (SELECT MaQuanLy FROM TinhThanh_New)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END