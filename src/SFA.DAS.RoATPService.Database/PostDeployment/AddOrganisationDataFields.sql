-- CSP-2267 adding fields from OrganisationData
	update Organisations set CompanyNumber =  JSON_VALUE(OrganisationData, '$.CompanyNumber'),
		CharityNumber = JSON_VALUE(OrganisationData, '$.CharityNumber'),
		RemovedReasonId = JSON_VALUE(OrganisationData, '$.RemovedReason.Id'),
		StartDate = JSON_VALUE(OrganisationData, '$.StartDate'),
		ApplicationDeterminedDate = JSON_VALUE(OrganisationData, '$.ApplicationDeterminedDate'),
		[ParentCompanyGuarantee] = CASE JSON_VALUE(OrganisationData,'$.ParentCompanyGuarantee')  WHEN 'true' THEN 1 WHEN 'false' THEN 0 ELSE null END,
		[FinancialTrackRecord] = CASE JSON_VALUE(OrganisationData,'$.FinancialTrackRecord')  WHEN 'true' THEN 1 WHEN 'false' THEN 0 ELSE null END,
		[NonLevyContract] = CASE JSON_VALUE(OrganisationData,'$.NonLevyContract')  WHEN 'true' THEN 1 WHEN 'false' THEN 0 ELSE null END
