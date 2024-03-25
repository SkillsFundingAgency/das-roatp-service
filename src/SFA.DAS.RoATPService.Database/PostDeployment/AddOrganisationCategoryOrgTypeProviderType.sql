GO

CREATE TABLE #TempOrganisationCategoryOrgTypeProviderType(
	[Id] int,
	[OrganisationCategoryId] [int] NOT NULL,
	[OrganisationTypeId] [int] NOT NULL,
	[ProviderTypeId] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](30) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
);

INSERT INTO #TempOrganisationCategoryOrgTypeProviderType
 ([Id],[OrganisationCategoryId],[OrganisationTypeId],ProviderTypeId,[CreatedAt],[CreatedBy],[Status])
	VALUES
	(1,1,1,1,GetDate(),'System','Live'),
	(2,1,1,2,GetDate(),'System','Live'),
	(3,1,1,3,GetDate(),'System','Live'),
	(4,1,2,1,GetDate(),'System','Live'),
	(5,1,2,2,GetDate(),'System','Live'),
	(6,1,2,3,GetDate(),'System','Live'),
	(7,1,3,1,GetDate(),'System','Live'),
	(8,1,3,2,GetDate(),'System','Live'),
	(9,1,3,3,GetDate(),'System','Live'),
	(10,1,4,1,GetDate(),'System','Live'),
	(11,1,4,2,GetDate(),'System','Live'),
	(12,1,4,3,GetDate(),'System','Live'),
	(13,1,5,1,GetDate(),'System','Live'),
	(14,1,5,2,GetDate(),'System','Live'),
	(15,1,5,3,GetDate(),'System','Live'),
	(16,1,6,1,GetDate(),'System','Live'),
	(17,1,6,2,GetDate(),'System','Live'),
	(18,1,6,3,GetDate(),'System','Live'),
	(19,1,7,1,GetDate(),'System','Live'),
	(20,1,7,2,GetDate(),'System','Live'),
	(21,1,7,3,GetDate(),'System','Live'),
	(22,1,8,1,GetDate(),'System','Live'),
	(23,1,8,2,GetDate(),'System','Live'),
	(24,1,8,3,GetDate(),'System','Live'),
	(25,2,9,1,GetDate(),'System','Live'),
	(26,2,9,2,GetDate(),'System','Live'),
	(27,2,9,3,GetDate(),'System','Live'),
	(28,2,10,1,GetDate(),'System','Live'),
	(29,2,10,2,GetDate(),'System','Live'),
	(30,2,10,3,GetDate(),'System','Live'),
	(31,2,11,1,GetDate(),'System','Live'),
	(32,2,11,2,GetDate(),'System','Live'),
	(33,2,11,3,GetDate(),'System','Live'),
	(34,2,12,1,GetDate(),'System','Live'),
	(35,2,12,2,GetDate(),'System','Live'),
	(36,2,12,3,GetDate(),'System','Live'),
	(37,2,13,1,GetDate(),'System','Live'),
	(38,2,13,2,GetDate(),'System','Live'),
	(39,2,13,3,GetDate(),'System','Live'),
	(40,2,14,1,GetDate(),'System','Live'),
	(41,2,14,2,GetDate(),'System','Live'),
	(42,2,14,3,GetDate(),'System','Live'),
	(43,2,15,1,GetDate(),'System','Live'),
	(44,2,15,2,GetDate(),'System','Live'),
	(45,2,15,3,GetDate(),'System','Live'),
	(46,16,16,1,GetDate(),'System','Live'),
	(47,16,16,3,GetDate(),'System','Live'),
	(48,17,17,1,GetDate(),'System','Live'),
	(49,17,17,3,GetDate(),'System','Live'),
	(50,18,18,1,GetDate(),'System','Live'),
	(51,18,18,3,GetDate(),'System','Live'),
	(52,19,19,1,GetDate(),'System','Live'),
	(53,19,19,3,GetDate(),'System','Live'),
	(54,3,20,2,GetDate(),'System','Live'),
	(55,21,21,1,GETUTCDATE(),'System','Live'),
	(56,21,21,2,GETUTCDATE(),'System','Live'),
	(57,21,21,3,GETUTCDATE(),'System','Live');

SET IDENTITY_INSERT [OrganisationCategoryOrgTypeProviderType] ON;
MERGE [OrganisationCategoryOrgTypeProviderType] TARGET
USING #TempOrganisationCategoryOrgTypeProviderType SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[OrganisationCategoryId] = SOURCE.[OrganisationCategoryId],
		TARGET.[OrganisationTypeId] = SOURCE.[OrganisationTypeId],
		TARGET.[ProviderTypeId] = SOURCE.[ProviderTypeId],
		TARGET.[CreatedAt] = SOURCE.[CreatedAt],
		TARGET.[CreatedBy] = SOURCE.[CreatedBy],
		TARGET.[Status] = SOURCE.[Status]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id],[OrganisationCategoryId],[OrganisationTypeId],[ProviderTypeId],[CreatedAt],[CreatedBy],[Status])
	VALUES (SOURCE.[Id], SOURCE.[OrganisationCategoryId],SOURCE.[OrganisationTypeId],SOURCE.[ProviderTypeId], SOURCE.[CreatedAt], SOURCE.[CreatedBy], SOURCE.[Status]);

SET IDENTITY_INSERT [OrganisationCategoryOrgTypeProviderType] OFF;

DROP TABLE #TempOrganisationCategoryOrgTypeProviderType;

GO