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


:r .\PostDeployment\UpdateTables.sql

Go
-- APR-474 update any non-set SourceIsUKRLP to true for Organisations.OrganisationData
update organisations set OrganisationData = JSON_Modify(OrganisationData,'$.SourceIsUKRLP',CAST(1 as BIT)) where JSON_VALUE(OrganisationData,'$.SourceIsUKRLP') is NULL;

Go