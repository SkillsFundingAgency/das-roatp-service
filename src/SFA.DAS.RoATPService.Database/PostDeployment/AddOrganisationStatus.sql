Go

CREATE TABLE #TempOrganisationStatus(
	[Id] [int],
	[Status] [nvarchar](50),
	[CreatedBy] [nvarchar](30),
	[CreatedAt] [datetime2](7),
	EventDescription [nvarchar](20) 
);


INSERT INTO #TempOrganisationStatus
	([Id], [Status], [CreatedBy], [CreatedAt],[EventDescription])
	VALUES
	(0, 'Removed', 'System', SYSDATETIME(),'REMOVED'),
	(1, 'Active', 'System', SYSDATETIME(),'ACTIVE'),
	(2, 'Active - but not taking on apprentices', 'System', SYSDATETIME(),'ACTIVENOSTARTS'),
	(3, 'On-boarding', 'System', getdate(),'INITIATED');

	
SET IDENTITY_INSERT [OrganisationStatus] ON;
MERGE [OrganisationStatus] TARGET
USING #TempOrganisationStatus SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[Status] = SOURCE.[Status],
		TARGET.[EventDescription] = SOURCE.[EventDescription]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Status], [CreatedBy], [CreatedAt],[EventDescription])
	VALUES (SOURCE.[Id], SOURCE.[Status], SOURCE.[CreatedBy], SOURCE.[CreatedAt], SOURCE.[EventDescription]);

SET IDENTITY_INSERT [OrganisationStatus] OFF;

DROP TABLE #TempOrganisationStatus;

Go