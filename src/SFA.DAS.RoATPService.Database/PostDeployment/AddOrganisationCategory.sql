CREATE TABLE #TempOrganisationCategory(
	[Id] [int],
	[Category] [nvarchar](100),
	[CreatedAt] [datetime2](7),
	[CreatedBy] [nvarchar](30),
	[Status] [nvarchar](20)
)


INSERT INTO #TempOrganisationCategory
	([Id],[Category],[CreatedAt],[CreatedBy],[Status])
	VALUES
	(1,'Educational institute',getdate(),'System','Live'),
	(2,'Public sector body',getdate(),'System','Live'),
	(16,'Independent training provider',getdate(),'System','Live'),
	(17,'Apprenticeship training agency',getdate(),'System','Live'),
	(18,'A Group Training Association',getdate(),'System','Live'),
	(19,'Employer',getdate(),'System','Live'),
	(3,'None of the above',getdate(),'System','Live'),
	(21,'Rail franchise',GETUTCDATE(),'System','Live')

SET IDENTITY_INSERT [OrganisationCategory] ON;
MERGE [OrganisationCategory] TARGET
USING #TempOrganisationCategory SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[Category] = SOURCE.[Category],
		TARGET.[CreatedAt] = SOURCE.[CreatedAt],
		TARGET.[CreatedBy] = SOURCE.[CreatedBy],
		TARGET.[Status] = SOURCE.[Status]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id],[Category],[CreatedAt],[CreatedBy],[Status])
	VALUES (SOURCE.[Id], SOURCE.[Category], SOURCE.[CreatedAt], SOURCE.[CreatedBy], SOURCE.[Status]);

SET IDENTITY_INSERT [OrganisationCategory] OFF;

DROP TABLE #TempOrganisationCategory
