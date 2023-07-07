CREATE PROCEDURE [dbo].[uspGetBlog]
(
	@Id INT	
)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		[Id],
		[Title],
		[Contents]		
	FROM [dbo].[Blogs]
	WHERE
		[Id] = @Id;	
END;
