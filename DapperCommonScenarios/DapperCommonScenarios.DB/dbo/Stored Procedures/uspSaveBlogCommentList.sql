CREATE PROCEDURE [dbo].[uspSaveBlogCommentList]
(
    @BlogId INT,
	@BlogCommentTable as [dbo].[BlogCommentTable] READONLY
)
AS
BEGIN
	-- Save the list using a MERGE statement
	MERGE [dbo].[BlogComments] AS [Target]
	USING @BlogCommentTable AS [Source]
	ON ([Target].[ID] = [Source].[ID])
	WHEN MATCHED THEN
		UPDATE SET	
			[BlogId] = @BlogId,
			[Comment] = [Source].[Comment],
			[NumberOfStars] = [Source].[NumberOfStars]
	WHEN NOT MATCHED THEN
		INSERT(
			[BlogId],
			[Comment],
			[NumberOfStars]
		) VALUES (
            @BlogId,
			[Source].[Comment],
			[Source].[NumberOfStars]
		);

	-- Get the list of objects
    SELECT
        [Id],
		[BlogId],
		[Comment],
		[NumberOfStars]
    FROM [dbo].[BlogComments]
    WHERE BlogId = @BlogId;
END;
