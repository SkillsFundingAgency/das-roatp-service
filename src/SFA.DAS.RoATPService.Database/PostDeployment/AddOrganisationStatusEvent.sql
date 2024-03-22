TRUNCATE TABLE [OrganisationStatusEvent]

INSERT INTO [dbo].[OrganisationStatusEvent]
           (OrganisationStatusId, CreatedOn,ProviderId)
	  select o.StatusId, o.StatusDate, o.ukprn from Organisations o 
	  left outer join OrganisationStatusEvent ose on o.ukprn = ose.providerId
	  and o.StatusId = ose.OrganisationStatusId and o.StatusDate = ose.CreatedOn
	  where ose.ProviderId is null and ose.OrganisationStatusId is null and ose.CreatedOn is null
	  and o.ProviderTypeId in (1,2)