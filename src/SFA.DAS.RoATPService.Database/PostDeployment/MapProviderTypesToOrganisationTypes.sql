Go

CREATE TABLE #TempProviderTypeOrganisationTypes(
	[Id] [int],
	[ProviderTypeId] [int],
	[OrganisationTypeId] [int],
	[CreatedAt] [datetime2](7),
	[CreatedBy] [nvarchar](30),
	[Status] [nvarchar](20)
);


INSERT INTO #TempProviderTypeOrganisationTypes
([Id], [ProviderTypeId], [OrganisationTypeId], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(1, 1, 0, 'System', SYSDATETIME(), 'Live'),
	(2, 2, 0, 'System', SYSDATETIME(), 'Live'),
	(3, 3, 0, 'System', SYSDATETIME(), 'Live'),
	(4, 1, 1, 'System', SYSDATETIME(), 'Live'),
	(5, 3, 1, 'System', SYSDATETIME(), 'Live'),
	(6, 1, 2, 'System', SYSDATETIME(), 'Live'),
	(7, 3, 2, 'System', SYSDATETIME(), 'Live'),
	(8, 1, 3, 'System', SYSDATETIME(), 'Live'),
	(9, 3, 3, 'System', SYSDATETIME(), 'Live'),
	(10, 1, 4, 'System', SYSDATETIME(), 'Live'),
	(11, 3, 4, 'System', SYSDATETIME(), 'Live'),
	(12, 1, 5, 'System', SYSDATETIME(), 'Live'),
	(13, 3, 5, 'System', SYSDATETIME(), 'Live'),
	(14, 1, 6, 'System', SYSDATETIME(), 'Live'),
	(15, 3, 6, 'System', SYSDATETIME(), 'Live'),
	(16, 1, 7, 'System', SYSDATETIME(), 'Live'),
	(17, 3, 7, 'System', SYSDATETIME(), 'Live'),
	(18, 1, 8, 'System', SYSDATETIME(), 'Live'),
	(19, 3, 8, 'System', SYSDATETIME(), 'Live'),
	(20, 1, 9, 'System', SYSDATETIME(), 'Live'),
	(21, 3, 9, 'System', SYSDATETIME(), 'Live'),
	(22, 1, 10, 'System', SYSDATETIME(), 'Live'),
	(23, 3, 10, 'System', SYSDATETIME(), 'Live'),
	(24, 1, 11, 'System', SYSDATETIME(), 'Live'),
	(25, 3, 11, 'System', SYSDATETIME(), 'Live'),
	(26, 1, 12, 'System', SYSDATETIME(), 'Live'),
	(27, 3, 12, 'System', SYSDATETIME(), 'Live'),
	(28, 1, 13, 'System', SYSDATETIME(), 'Live'),
	(29, 3, 13, 'System', SYSDATETIME(), 'Live'),
	(30, 1, 14, 'System', SYSDATETIME(), 'Live'),
	(31, 3, 14, 'System', SYSDATETIME(), 'Live'),
	(32, 1, 15, 'System', SYSDATETIME(), 'Live'),
	(33, 3, 15, 'System', SYSDATETIME(), 'Live'),
	(34, 1, 16, 'System', SYSDATETIME(), 'Live'),
	(35, 3, 16, 'System', SYSDATETIME(), 'Live'),
	(36, 1, 17, 'System', SYSDATETIME(), 'Live'),
	(37, 3, 17, 'System', SYSDATETIME(), 'Live'),
	(38, 1, 18, 'System', SYSDATETIME(), 'Live'),
	(39, 3, 18, 'System', SYSDATETIME(), 'Live'),
	(40, 1, 19, 'System', SYSDATETIME(), 'Live'),
	(41, 3, 19, 'System', SYSDATETIME(), 'Live'),
	(42, 2, 1, 'System', SYSDATETIME(), 'Live'),
	(43, 2, 2, 'System', SYSDATETIME(), 'Live'),
	(44, 2, 3, 'System', SYSDATETIME(), 'Live'),
	(45, 2, 4, 'System', SYSDATETIME(), 'Live'),
	(46, 2, 5, 'System', SYSDATETIME(), 'Live'),
	(47, 2, 6, 'System', SYSDATETIME(), 'Live'),
	(48, 2, 7, 'System', SYSDATETIME(), 'Live'),
	(49, 2, 8, 'System', SYSDATETIME(), 'Live'),
	(50, 2, 9, 'System', SYSDATETIME(), 'Live'),
	(51, 2, 10, 'System', SYSDATETIME(), 'Live'),
	(52, 2, 11, 'System', SYSDATETIME(), 'Live'),
	(53, 2, 12, 'System', SYSDATETIME(), 'Live'),
	(54, 2, 13, 'System', SYSDATETIME(), 'Live'),
	(55, 2, 14, 'System', SYSDATETIME(), 'Live'),
	(56, 2, 15, 'System', SYSDATETIME(), 'Live'),
	(57, 2, 20, 'System', SYSDATETIME(), 'Live'),
	(58, 1, 21, 'System', GETUTCDATE(), 'Live'),
	(59, 2, 21, 'System', GETUTCDATE(), 'Live'),
	(60, 3, 21, 'System', GETUTCDATE(), 'Live');

SET IDENTITY_INSERT [ProviderTypeOrganisationTypes] ON;
MERGE [ProviderTypeOrganisationTypes] TARGET
USING #TempProviderTypeOrganisationTypes SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[ProviderTypeId] = SOURCE.[ProviderTypeId],
		TARGET.[OrganisationTypeId] = SOURCE.[OrganisationTypeId],
		TARGET.[Status] = SOURCE.[Status]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [ProviderTypeId], [OrganisationTypeId], [CreatedBy], [CreatedAt], [Status])
	VALUES (SOURCE.[Id], SOURCE.[ProviderTypeId], SOURCE.[OrganisationTypeId], SOURCE.[CreatedBy], SOURCE.[CreatedAt], SOURCE.[Status]);

SET IDENTITY_INSERT [ProviderTypeOrganisationTypes] OFF;

DROP TABLE #TempProviderTypeOrganisationTypes;

GO