CREATE PROCEDURE [dbo].[RoATP_CSV_SUMMARY]
    (@ukprn INT = null)
AS

SET NOCOUNT ON

SELECT UKPRN AS UKPRN, 
 LegalName + 
     CASE ISNULL(TradingName,'') WHEN '' THEN ''
     ELSE ' T/A ' + TradingName
     END AS 'Organisation Name',
 pt.ProviderType AS 'Application Type',
 CASE o.NonLevyContract
    WHEN 1 THEN 'Y' ELSE 'N' END  AS 'Contracted to deliver to non-levied employers',
 CONVERT(VARCHAR(10),o.StartDate, 111) AS  'Start Date',
 CASE StatusId WHEN 0 THEN CONVERT(VARCHAR(10),StatusDate,111) ELSE NULL END AS 'End Date',
 CASE StatusId WHEN 2 THEN CONVERT(VARCHAR(10),StatusDate,111) ELSE NULL END AS 'Provider not currently starting new apprentices',
 CONVERT(VARCHAR(10),o.ApplicationDeterminedDate, 111) AS  'Application Determined Date'
 FROM Organisations o 
 LEFT OUTER JOIN ProviderTypes pt ON o.ProviderTypeId = pt.Id
     WHERE o.StatusId IN (0,1,2) -- exclude on-boarding
      AND UKPRN = ISNULL(@ukprn, UKPRN)
      ORDER BY COALESCE(o.UpdatedAt, o.CreatedAt) DESC