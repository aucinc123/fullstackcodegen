CREATE TABLE [dbo].[BlogComments]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[BlogId] INT NOT NULL,
	[Comment] VARCHAR(1000) NOT NULL,
	[NumberOfStars] INT NULL,
	CONSTRAINT [FK_BlogComments_Blog_Id] FOREIGN KEY (BlogId) REFERENCES [dbo].[Blogs](Id)
);
