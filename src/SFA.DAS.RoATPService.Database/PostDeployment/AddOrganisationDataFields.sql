-- CSP-2267 adding fields from OrganisationData
-- THIS CAN BE REMOVED (AND postDeployment to call it, removed too) once endpoints are writing to these 5 fields
	if exists(select * from organisations where startDate is null)
		BEGIN
			update Organisations set CompanyNumber =  JSON_VALUE(OrganisationData, '$.CompanyNumber'),
				CharityNumber = JSON_VALUE(OrganisationData, '$.CharityNumber'),
				RemovedReasonId = JSON_VALUE(OrganisationData, '$.RemovedReason.Id'),
				StartDate = JSON_VALUE(OrganisationData, '$.StartDate'),
				ApplicationDeterminedDate = JSON_VALUE(OrganisationData, '$.ApplicationDeterminedDate')
		END
