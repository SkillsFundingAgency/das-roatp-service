CREATE TABLE #TempCourseTypes(
    [Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [IsActive] BIT NOT NULL
);

INSERT INTO #TempCourseTypes (Id, Name, IsActive) VALUES 
    (1, 'Apprenticeship', 1), 
    (2, 'ShortCourse', 1);

MERGE INTO dbo.CourseTypes AS target
USING #TempCourseTypes AS source
ON target.Id = source.Id
WHEN MATCHED THEN 
    UPDATE SET target.Name = source.Name, target.IsActive = source.IsActive
WHEN NOT MATCHED BY TARGET THEN 
    INSERT (Id, Name, IsActive) VALUES (source.Id, source.Name, source.IsActive);

DROP TABLE #TempCourseTypes;
