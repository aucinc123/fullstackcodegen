CREATE PROCEDURE [dbo].[uspGetBlogWithComments]
(
	@Id INT	
)
AS
BEGIN
	SET NOCOUNT ON;
	-- Get Blog object
	SELECT 
		[Id],
		[Title],
		[Contents]		
	FROM [dbo].[Blogs]
	WHERE
		[Id] = @Id;	

	-- Get List of Blog Comments
	SELECT 
		[Id],
		[BlogId],
		[Comment],
		[NumberOfStars]		
	FROM [dbo].[BlogComments]
	WHERE
		[BlogId] = @Id;
END;
