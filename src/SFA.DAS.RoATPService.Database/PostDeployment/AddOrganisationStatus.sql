CREATE TABLE #TempOrganisationStatus(
	[Id] [int],
	[Status] [nvarchar](50),
	[CreatedBy] [nvarchar](30),
	[CreatedAt] [datetime2](7)
)


INSERT INTO #TempOrganisationStatus
	([Id], [Status], [CreatedBy], [CreatedAt])
	VALUES
	(0, 'REMOVED', 'System', SYSDATETIME()),
	(1, 'ACTIVE', 'System', SYSDATETIME()),
	(2, 'ACTIVENOSTARTS', 'System', SYSDATETIME()),
	(3, 'INITIATED', 'System', getdate())

	
SET IDENTITY_INSERT [OrganisationStatus] ON;
MERGE [OrganisationStatus] TARGET
USING #TempOrganisationStatus SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[Status] = SOURCE.[Status],
		TARGET.[CreatedBy] = SOURCE.[CreatedBy],
		TARGET.[CreatedAt] = SOURCE.[CreatedAt]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Status], [CreatedBy], [CreatedAt])
	VALUES (SOURCE.[Id], SOURCE.[Status], SOURCE.[CreatedBy], SOURCE.[CreatedAt]);

SET IDENTITY_INSERT [OrganisationStatus] OFF;

DROP TABLE #TempOrganisationStatus