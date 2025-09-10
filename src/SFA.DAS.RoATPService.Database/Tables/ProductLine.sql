CREATE TABLE [dbo].[ProductLine]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL,
    CONSTRAINT [UQ_ProductLine_Name] UNIQUE (Name)
)