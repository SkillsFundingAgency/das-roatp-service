CREATE TABLE #TempProviderTypeOrganisationStatus(
	[Id] [int],
	[ProviderTypeId] [int],
	[OrganisationStatusId] [int],
	[CreatedAt] [datetime2](7),
	[CreatedBy] [nvarchar](30),
	[Status] [nvarchar](20)
)


INSERT INTO #TempProviderTypeOrganisationStatus
	([Id], [ProviderTypeId], [OrganisationStatusId], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1, 1, 0, 'System', SYSDATETIME(), 'Live'),
	(2, 2, 0, 'System', SYSDATETIME(), 'Live'),
	(3, 3, 0, 'System', SYSDATETIME(), 'Live'),
	(4, 1, 1, 'System', SYSDATETIME(), 'Live'),
	(5, 2, 1, 'System', SYSDATETIME(), 'Live'),
	(6, 3, 1, 'System', SYSDATETIME(), 'Live'),
	(7, 1, 2, 'System', SYSDATETIME(), 'Live'),
	(8, 2, 2, 'System', SYSDATETIME(), 'Live'),
	(9, 3, 2, 'System', SYSDATETIME(), 'Live'),
	(10, 1, 3, 'System', SYSDATETIME(), 'Live'),
	(11, 2, 3, 'System', SYSDATETIME(), 'Live')

	
SET IDENTITY_INSERT [ProviderTypeOrganisationStatus] ON;
MERGE [ProviderTypeOrganisationStatus] TARGET
USING #TempProviderTypeOrganisationStatus SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[ProviderTypeId] = SOURCE.[ProviderTypeId],
		TARGET.[OrganisationStatusId] = SOURCE.[OrganisationStatusId],
		TARGET.[CreatedBy] = SOURCE.[CreatedBy],
		TARGET.[CreatedAt] = SOURCE.[CreatedAt],
		TARGET.[Status] = SOURCE.[Status]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [ProviderTypeId], [OrganisationStatusId], [CreatedBy], [CreatedAt], [Status])
	VALUES (SOURCE.[Id], SOURCE.[ProviderTypeId], SOURCE.[OrganisationStatusId], SOURCE.[CreatedBy], SOURCE.[CreatedAt], SOURCE.[Status]);

SET IDENTITY_INSERT [ProviderTypeOrganisationStatus] OFF;

DROP TABLE #TempProviderTypeOrganisationStatus