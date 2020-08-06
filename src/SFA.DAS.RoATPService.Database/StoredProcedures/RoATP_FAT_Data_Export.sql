CREATE PROCEDURE dbo.RoATP_FAT_Data_Export
AS
BEGIN
SELECT 
	o.UKPRN,
	o.StatusDate,
	o.StatusId,
	o.OrganisationTypeId,
	o.ProviderTypeId
	FROM Organisations o
	ORDER BY o.LegalName ASC
END
GO
