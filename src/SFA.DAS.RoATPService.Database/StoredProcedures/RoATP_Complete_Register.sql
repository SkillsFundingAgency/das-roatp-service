CREATE PROCEDURE dbo.RoATP_Complete_Register
AS
BEGIN
SELECT 
	pt.ProviderType AS [Provider type],
	o.UKPRN,
	o.LegalName AS [Legal name],
	o.TradingName AS [Trading name],
	ot.Type AS [Organisation type],
	o.CompanyNumber AS [Company number],
	o.CharityNumber AS [Charity number],
	CASE o.ParentCompanyGuarantee
	WHEN 1 THEN 'Yes'
	ELSE 'No'
	END AS [Parent company guarantee],
	CASE o.FinancialTrackRecord
	WHEN 1 THEN 'Yes'
	ELSE 'No'
	END AS [Financial track record],
	os.[Status],
	SUBSTRING(CONVERT(VARCHAR, o.StatusDate, 103), 0, 11) AS [Status date],
	rr.RemovedReason AS [Reason],
	SUBSTRING(CONVERT(VARCHAR, o.StartDate, 103), 0, 11) 
	AS [Date joined]
	FROM Organisations o
	INNER JOIN ProviderTypes pt
	ON pt.Id = o.ProviderTypeId
	INNER JOIN OrganisationTypes ot
	ON ot.Id = o.OrganisationTypeId
	INNER JOIN OrganisationStatus os
	ON os.Id = o.StatusId
	LEFT OUTER JOIN RemovedReasons rr
	ON o.RemovedReasonId = rr.Id
	ORDER BY o.LegalName ASC
END
GO