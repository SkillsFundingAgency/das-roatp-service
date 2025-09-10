CREATE TABLE #TempProductLines
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL
);

INSERT INTO #TempProductLines (Id, Name) VALUES 
    (1, 'Apprenticeships'), 
    (2, 'ShortCourses');

MERGE INTO dbo.ProductLine AS target
USING #TempProductLines AS source
ON target.Id = source.Id
WHEN MATCHED THEN 
    UPDATE SET target.Name = source.Name
WHEN NOT MATCHED BY TARGET THEN 
    INSERT (Id, Name) VALUES (source.Id, source.Name);
    
DROP TABLE #TempProductLines;
