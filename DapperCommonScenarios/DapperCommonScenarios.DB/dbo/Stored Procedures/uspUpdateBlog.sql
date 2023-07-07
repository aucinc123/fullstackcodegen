CREATE PROCEDURE [dbo].[uspUpdateBlog]
(
	@Id INT,
	@Title VARCHAR(100),
	@Contents VARCHAR(max)
)
AS
BEGIN
	DECLARE @DoesExist bit;
	SET @DoesExist = 0;

	IF EXISTS(SELECT [Id] FROM Blogs WHERE [Id] = @Id)
	BEGIN
		SET @DoesExist = 1;
		UPDATE [dbo].[Blogs]
		SET 
			[Title] = @Title,
			[Contents] = @Contents 
		WHERE 
			[Id] = @Id;
	END;

	SELECT @DoesExist;
END;
