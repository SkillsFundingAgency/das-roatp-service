DELETE OrganisationCourseTypes

INSERT INTO OrganisationCourseTypes (Id, OrganisationId, CourseTypeId, HasAccess)
    SELECT NEWID(), Id, 1, 1
    FROM dbo.Organisations
    where ProviderTypeId in (1, 2) -- Main Providers and Employer Providers

