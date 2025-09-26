CREATE TABLE #TempCourseTypes(
    [Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [LearningType] NVARCHAR(20) NOT NULL, 
    [IsActive] BIT NOT NULL
);

INSERT INTO #TempCourseTypes (Id, Name, LearningType, IsActive) VALUES 
    (1, 'Apprenticeship', 'Standard', 1), 
    (2, 'Unit', 'ShortCourse', 1);

MERGE INTO dbo.CourseTypes AS target
USING #TempCourseTypes AS source
ON target.Id = source.Id
WHEN MATCHED THEN 
    UPDATE SET target.Name = source.Name, target.LearningType = source.LearningType, target.IsActive = source.IsActive
WHEN NOT MATCHED BY TARGET THEN 
    INSERT (Id, Name, LearningType, IsActive) VALUES (source.Id, source.Name, source.LearningType, source.IsActive);

DROP TABLE #TempCourseTypes;
