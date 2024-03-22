CREATE TABLE #TempOrganisationTypes(
	[Id] [int],
	[Type] [nvarchar](100),
	[Description] [nvarchar](255),
	[CreatedAt] [datetime2](7),
	[CreatedBy] [nvarchar](30),
	[UpdatedAt] [datetime2](7),
	[UpdatedBy] [nvarchar](30),
	[Status] [nvarchar](20)
)

INSERT INTO #TempOrganisationTypes
	([Id], [Type], [CreatedBy], [CreatedAt], [Status])
	VALUES
	(0, 'Unassigned', 'System', SYSDATETIME(), 'Live'),
	(1, 'School', 'System', SYSDATETIME(), 'Live'),
	(2, 'General Further Education College', 'System', SYSDATETIME(), 'Live'),
	(3, 'National College', 'System', SYSDATETIME(), 'Live'),
	(4, 'Sixth Form College', 'System', SYSDATETIME(), 'Live'),
	(5, 'Further Education Institute', 'System', SYSDATETIME(), 'Live'),
	(6, 'Higher Education Institute or university', 'System', SYSDATETIME(), 'Live'),
	(7, 'Academy', 'System', SYSDATETIME(), 'Live'),
	(8, 'Multi-Academy Trust', 'System', SYSDATETIME(), 'Live'),
	(9, 'NHS Trust', 'System', SYSDATETIME(), 'Live'),
	(10, 'Police', 'System', SYSDATETIME(), 'Live'),
	(11, 'Fire authority', 'System', SYSDATETIME(), 'Live'),
	(12, 'Local authority', 'System', SYSDATETIME(), 'Live'),
	(13, 'Government department', 'System', SYSDATETIME(), 'Live'),
	(14, 'Non-departmental public body (NDPB)', 'System', SYSDATETIME(), 'Live'),
	(15, 'Executive agency', 'System', SYSDATETIME(), 'Live'),
	(16, 'An Independent Training Provider', 'System', SYSDATETIME(), 'Live'),
	(17, 'An Apprenticeship Training Provider', 'System', SYSDATETIME(), 'Live'),
	(18, 'A Group Training Association', 'System', SYSDATETIME(), 'Live'),
	(19, 'An employer training apprentices in other organisations', 'System', SYSDATETIME(), 'Live'),
	(20, 'None of the above', 'System', SYSDATETIME(), 'Live'),
	(21, 'Rail franchise','Live',GETUTCDATE(),'System')

SET IDENTITY_INSERT [OrganisationTypes] ON;
MERGE [OrganisationTypes] TARGET
USING #TempOrganisationTypes SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[Type] = SOURCE.[Type],
		TARGET.[CreatedAt] = SOURCE.[CreatedAt],
		TARGET.[CreatedBy] = SOURCE.[CreatedBy],
		TARGET.[Status] = SOURCE.[Status]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Type], [CreatedBy], [CreatedAt], [Status])
	VALUES (SOURCE.[Id], SOURCE.[Type], SOURCE.[CreatedBy], SOURCE.[CreatedAt], SOURCE.[Status]);

SET IDENTITY_INSERT [OrganisationTypes] OFF;

DROP TABLE #TempOrganisationTypes
