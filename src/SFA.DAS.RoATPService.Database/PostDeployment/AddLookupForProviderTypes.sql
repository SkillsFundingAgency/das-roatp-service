CREATE TABLE #TempProviderTypes(
	[Id] [int],
	[ProviderType] [nvarchar](100),
	[Description] [nvarchar](max),
	[CreatedAt] [datetime2](7),
	[CreatedBy] [nvarchar](30),
	[UpdatedAt] [datetime2](7),
	[UpdatedBy] [nvarchar](30),
	[Status] [nvarchar](20),
)


INSERT INTO #TempProviderTypes
([Id], [ProviderType], [CreatedBy], [CreatedAt], [Status],[Description])
	VALUES
	(1,
	'Main provider', 'System', SYSDATETIME(), 'Live','<p>Your organisation will directly deliver training to apprentices for other organisations.</p> <p>While doing this, it can also train:</p> <ul class=''govuk-list govuk-list--bullet govuk-hint''> <li>its own employees</li> <li>employees of connected companies or charities</li> <li>act as a subcontractor for other main and employer providers</li> </ul>'),
	(2,
	'Employer provider', 'System', SYSDATETIME(), 'Live','<p>Your organisation will directly deliver training to its own employees.</p> <p>While doing this, it can also train:</p> <ul class=''govuk-list govuk-list--bullet govuk-hint''> <li>employees of connected companies or charities</li> <li>act as a subcontractor for other employer or main provider</li> </ul> <p>To be an employer provider, your organisation must be a levy-paying employer.</p>'),
	(3,
	'Supporting provider', 'System', SYSDATETIME(), 'Live','<p>Your organisation will act as a subcontractor for main and employer providers to train apprentices up to a maximum of &pound;500,000 per year.</p> <p>If your organisation is new on the register, it will be limited to &pound;100,000 in its first year.</p>')

SET IDENTITY_INSERT [ProviderTypes] ON;
MERGE [ProviderTypes] TARGET
USING #TempProviderTypes SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[ProviderType] = SOURCE.[ProviderType],
		TARGET.[CreatedBy] = SOURCE.[CreatedBy],
		TARGET.[CreatedAt] = SOURCE.[CreatedAt],
		TARGET.[Status] = SOURCE.[Status],
		TARGET.[Description] = SOURCE.[Description]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [ProviderType], [CreatedBy], [CreatedAt], [Status],[Description])
	VALUES (SOURCE.[Id], SOURCE.[ProviderType], SOURCE.[CreatedBy], SOURCE.[CreatedAt], SOURCE.[Status], SOURCE.[Description]);

SET IDENTITY_INSERT [ProviderTypes] OFF;

DROP TABLE #TempProviderTypes
