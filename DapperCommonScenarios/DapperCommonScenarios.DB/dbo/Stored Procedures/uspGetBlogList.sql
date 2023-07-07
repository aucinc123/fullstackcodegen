CREATE PROCEDURE [dbo].[uspGetBlogList]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		[Id],
		[Title],
		[Contents]		
	FROM [dbo].[Blogs];
END;
