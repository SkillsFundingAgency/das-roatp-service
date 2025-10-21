-- CSP-2267 adding fields from OrganisationData
	update Organisations set CompanyNumber =  JSON_VALUE(OrganisationData, '$.CompanyNumber'),
		CharityNumber = JSON_VALUE(OrganisationData, '$.CharityNumber'),
		RemovedReasonId = JSON_VALUE(OrganisationData, '$.RemovedReason.Id'),
		StartDate = JSON_VALUE(OrganisationData, '$.StartDate'),
		ApplicationDeterminedDate = JSON_VALUE(OrganisationData, '$.ApplicationDeterminedDate')
