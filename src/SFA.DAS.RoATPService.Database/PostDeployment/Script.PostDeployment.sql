/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\AddLookupForProviderTypes.sql

:r .\AddLookupForOrganisationTypes.sql

:r .\MapProviderTypesToOrganisationTypes.sql

:r .\AddRemovedReasons.sql

:r .\AddOrganisationStatus.sql

:r .\AddProviderTypeOrganisationStatus.sql

:r .\UpdateOrganisation.sql

:r .\AddOrganisationCategory.sql

:r .\AddOrganisationCategoryOrgTypeProviderType.sql

:r .\AddOrganisationStatusEvent.sql