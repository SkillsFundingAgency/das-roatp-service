CREATE TABLE #TempProduct(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [ProductLineId] INT NOT NULL, 
    [IsActive] BIT NOT NULL
);

INSERT INTO #TempProduct (Id, Name, ProductLineId, IsActive) VALUES 
    ('1f048e6d-6d18-460a-a5cf-20fc039e3d54', 'Apprenticeships', 1, 1), 
    ('a3fe9181-5ca1-4b51-9e0e-221f80b1fa18', 'Modules', 2, 1);

MERGE INTO dbo.Product AS target
USING #TempProduct AS source
ON target.Id = source.Id
WHEN MATCHED THEN 
    UPDATE SET target.Name = source.Name, target.ProductLineId = source.ProductLineId, target.IsActive = source.IsActive
WHEN NOT MATCHED BY TARGET THEN 
    INSERT (Id, Name, ProductLineId, IsActive) VALUES (source.Id, source.Name, source.ProductLineId, source.IsActive);

DROP TABLE #TempProduct;
