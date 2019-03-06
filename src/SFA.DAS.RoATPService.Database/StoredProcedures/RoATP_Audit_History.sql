﻿CREATE PROCEDURE [dbo].[RoATP_Audit_History]
AS
	SELECT
	  UKPRN,
	  LegalName AS [Legal name]
	  ,[FieldChanged] AS [Field of change]
	  ,[PreviousValue] AS [Old value]
	  ,CASE WHEN [newPreviousStatusDate] = '0' THEN CreatedAT ELSE [newPreviousStatusDate] END AS [Old status date]
	  ,[NewValue] AS [New value]
      ,FORMAT([UpdatedAt], 'dd/MM/yyyy HH:mm:ss') AS [Change date time]
      ,[UpdatedBy] AS [Who]
	FROM (
	SELECT 
		au1.*, og1.LegalName, og1.UKPRN
		,LAG(Convert(nvarchar, FORMAT(au1.UpdatedAt, 'dd/MM/yyyy HH:mm:ss')), 1,0) OVER (PARTITION BY au1.Organisationid ORDER BY au1.UpdatedAt ) AS newPreviousStatusDate,
		Convert(nvarchar, FORMAT(og1.createdAt, 'dd/MM/yyyy HH:mm:ss')) createdAt
		FROM [Audit] au1
		LEFT JOIN Organisations og1 ON og1.Id = au1.OrganisationId
	) ab1
	ORDER BY LegalName, ab1.Updatedat desc
GO
