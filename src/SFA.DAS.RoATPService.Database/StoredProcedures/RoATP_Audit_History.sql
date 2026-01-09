CREATE PROCEDURE [dbo].[RoATP_Audit_History]
AS
    SELECT
      Ukprn,
      LegalName,
      FieldChanged,
      PreviousValue,
      CASE FieldChanged 
        WHEN 'Organisation Status' THEN
          CASE 
            WHEN [HasPreviousStatusDate] = '0' THEN CreatedAt 
            ELSE [HasPreviousStatusDate]
          END
        ELSE null
      END AS [PreviousStatusDate],
      NewValue,
      [UpdatedAt],
      [UpdatedBy]
    FROM (
        SELECT distinct
            au1.*, og1.LegalName, og1.UKPRN,
            LAG(Convert(nvarchar, au1.UpdatedAt), 1,0) OVER (PARTITION BY au1.OrganisationId ORDER BY au1.UpdatedAt ) AS HasPreviousStatusDate,
            JSON_VALUE(jsonValue.Value, '$.FieldChanged') AS FieldChanged,
            JSON_VALUE(jsonValue.Value, '$.PreviousValue') AS PreviousValue,
            JSON_VALUE(jsonValue.Value, '$.NewValue') AS NewValue,
            og1.CreatedAt
        FROM [Audit] au1
        CROSS APPLY OPENJSON(au1.[AuditData], '$.FieldChanges') jsonValue
        LEFT JOIN Organisations og1 ON og1.Id = au1.OrganisationId
    ) ab1
    ORDER BY LegalName, ab1.UpdatedAt desc
GO
