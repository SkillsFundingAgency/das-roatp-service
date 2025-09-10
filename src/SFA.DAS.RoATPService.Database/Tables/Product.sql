CREATE TABLE [dbo].[Product]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [ProductLineId] INT NOT NULL, 
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [UQ_Product_Name] UNIQUE (Name),
    CONSTRAINT [FK_Product_ProductLine] FOREIGN KEY ([ProductLineId]) REFERENCES [ProductLine]([Id])
)
