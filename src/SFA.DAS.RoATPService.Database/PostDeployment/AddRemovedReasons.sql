CREATE TABLE #TempRemovedReasons(
	[Id] [int],
	[Status] [nvarchar](20),
	[RemovedReason] [nvarchar](100),
	[CreatedAt] [datetime2](7),
	[CreatedBy] [nvarchar](30),
);


INSERT INTO #TempRemovedReasons
([Id], [Status], [RemovedReason], [CreatedBy], [CreatedAt])
	VALUES
	(1, 'Live', 'Breach', 'System', SYSDATETIME()),
	(2, 'Live', 'Change of trading status', 'System', SYSDATETIME()),
	(3, 'Live', 'High risk policy', 'System', SYSDATETIME()),
	(4, 'Live', 'Inadequate financial health', 'System', SYSDATETIME()),
	(5, 'Live', 'Inadequate Ofsted grade', 'System', SYSDATETIME()),
	(6, 'Live', 'Internal error', 'System', SYSDATETIME()),
	(7, 'Live', 'Merger', 'System', SYSDATETIME()),
	(8, 'Live', 'Minimum standards not met', 'System', SYSDATETIME()),
	(9, 'Live', 'Non-direct delivery in 12 month period', 'System', SYSDATETIME()),
	(10, 'Live', 'Provider error', 'System', SYSDATETIME()),
	(11, 'Live', 'Provider request', 'System', SYSDATETIME()),
	(12, 'Live', 'Other', 'System', SYSDATETIME()),
	(13, 'Live', 'No delivery in a 6 month period', 'System', SYSDATETIME()),
	(14, 'Live', '2 insufficient progress Ofsted monitoring', 'System', SYSDATETIME()),
	(15, 'Live', 'Failed APAR application', 'System', SYSDATETIME()),
	(16, 'Live', 'Did not re-apply when requested', 'System', SYSDATETIME()),
	(17, 'Live', 'Gap In Provision - Condition 5 Breach', 'System', SYSDATETIME()),
	(18, 'Live', 'Apprenticeship Accountability Framework', 'System', SYSDATETIME());

	
SET IDENTITY_INSERT [RemovedReasons] ON;
MERGE [RemovedReasons] TARGET
USING #TempRemovedReasons SOURCE ON TARGET.Id=SOURCE.Id
WHEN MATCHED THEN
	UPDATE SET
		TARGET.[Status] = SOURCE.[Status],
		TARGET.[RemovedReason] = SOURCE.[RemovedReason]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Status], [RemovedReason], [CreatedBy], [CreatedAt])
	VALUES (SOURCE.[Id], SOURCE.[Status], SOURCE.[RemovedReason], SOURCE.[CreatedBy], SOURCE.[CreatedAt]);

SET IDENTITY_INSERT [RemovedReasons] OFF;

DROP TABLE #TempRemovedReasons;

GO