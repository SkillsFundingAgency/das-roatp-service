CREATE TABLE [dbo].[CourseTypes]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [LearningType] NVARCHAR(20) NOT NULL, 
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [UQ_CourseTypes_Name] UNIQUE (Name)
)
