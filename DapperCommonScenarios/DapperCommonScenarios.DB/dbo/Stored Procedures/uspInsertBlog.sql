CREATE PROCEDURE [dbo].[uspInsertBlog]
(
	@Title VARCHAR(100),
	@Contents VARCHAR(max)
)
AS
BEGIN
	INSERT INTO [dbo].[Blogs]
	(
		[Title],
		[Contents]
	) VALUES (
		@Title,
		@Contents
	);

	SELECT SCOPE_IDENTITY();	
END;
