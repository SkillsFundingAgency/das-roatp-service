-- Script to add new providers for short courses to das-prd-roatp-db
--
BEGIN TRANSACTION PAC;
BEGIN

CREATE TABLE #ProviderApproved
(
[Ukprn] bigint not null
);
CREATE UNIQUE INDEX IXU_Provider ON #ProviderApproved ([Ukprn]);

-- add Ukprns for provider approved for Short Courses (i.e. Apprenticeship Units)
INSERT INTO #ProviderApproved VALUES
----------------------------------------------------------------------------
-- EXAMPLES (to be removed)
----------------------------------------------------------------------------
(10000020),
(10000028),
(10000488),
(10000239),
(10002111),
(10090480),
(10000427);

----------------------------------------------------------------------------
-- EXAMPLES
----------------------------------------------------------------------------
;

SELECT COUNT(*) 'Before: Providers Approved for Short Courses ' FROM [dbo].[OrganisationCourseTypes] WHERE [CourseTypeId] = 2;
SELECT * FROM [dbo].[OrganisationCourseTypes] WHERE [CourseTypeId] = 2 ORDER BY 1;

-- Add any new Approved providers for shortcourses
INSERT INTO [dbo].[OrganisationCourseTypes] ([Id],[OrganisationId],[CourseTypeId])
SELECT NEWID(), org.[Id], 2
FROM #ProviderApproved tmp
JOIN [dbo].[Organisations] org on org.UKPRN = tmp.[Ukprn]
WHERE 1=1
AND NOT EXISTS (SELECT null FROM [OrganisationCourseTypes] WHERE [OrganisationId] = org.[Id] AND [CourseTypeId] = 2)
;

SELECT COUNT(*) 'After: Providers Approved for Short Courses ' FROM [dbo].[OrganisationCourseTypes] WHERE [CourseTypeId] = 2;
SELECT * FROM [dbo].[OrganisationCourseTypes] WHERE [CourseTypeId] = 2 ORDER BY 1;

DROP TABLE #ProviderApproved;

COMMIT TRANSACTION PAC;

END;
