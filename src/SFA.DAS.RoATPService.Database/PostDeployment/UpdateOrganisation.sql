--Go

--update organisations set OrganisationData = JSON_Modify(OrganisationData,'$.SourceIsUKRLP',CAST(1 as BIT)) where JSON_VALUE(OrganisationData,'$.SourceIsUKRLP') is NULL;

--GO