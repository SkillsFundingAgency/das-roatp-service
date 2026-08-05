# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  Register of Apprenticeship Training Providers (RoATP) API

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status%2FApprenticeships%20Providers%2Fdas-roatp-service?repoName=SkillsFundingAgency%2Fdas-roatp-service&branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1399&repoName=SkillsFundingAgency%2Fdas-roatp-service&branchName=master)
[![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-service&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-roatp-service)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


This API encapsulates ROATP Organisation data. This repo and its contained appare is in the process of being deprecated in favour of [das-roatp-api](https://github.com/SkillsFundingAgency/das-roatp-api) 


### Developer Setup

### Pre-Requisites

* A clone of this repository
* A storage emulator like Azurite
* Visual studio or similar IDE 
* Administrator Access
* Access to an Azure Service Bus namespace/connection string for event publishing

#### Setup

- Create a Configuration table in your (Development) local storage account.
- Obtain the local config json from the das-employer-config repo (<https://github.com/SkillsFundingAgency/das-employer-config>) and adjust the `SqlConnectionString` property to match your local setup
- Ensure the local config json also includes `AzureWebJobsServiceBus` (Service Bus connection string) so `ProviderRemovedEvent` messages can be published
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.RoATPService_1.0
  - Data: {The contents of the local config json file}
  
- In the web project, if not exist already, add `AppSettings.Development.json` file with following content:
```json  
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "RegisterAuditLogSettings": {
    "IgnoredFields": [
      "Id",
      "CreatedBy",
      "CreatedAt",
      "UpdatedBy",
      "UpdatedAt",
      "ProviderType.Id",
      "ProviderType.CreatedAt",
      "ProviderType.CreatedBy",
      "ProviderType.UpdatedBy",
      "ProviderType.UpdatedAt",
      "RemovedReason.Id",
      "OrganisationType.Id",
      "OrganisationType.CreatedAt",
      "OrganisationStatus.CreatedAt",
      "OrganisationData.RemovedReason.CreatedBy",
      "OrganisationData.RemovedReason.CreatedAt",
      "OrganisationData.RemovedReason.UpdatedBy",
      "OrganisationData.RemovedReason.UpdatedAt"
    ],
    "DisplayNames": [
      {
        "FieldName": "\"LegalName",
        "DisplayName": "Legal Name"
      },
      {
        "FieldName": "TradingName",
        "DisplayName": "Trading Name"
      },
      {
        "FieldName": "StatusDate",
        "DisplayName": "Status Effective From Date"
      },
      {
        "FieldName": "ProviderType.Type",
        "DisplayName": "Provider Type"
      },
      {
        "FieldName": "OrganisationData.RemovedReason.EndReason",
        "DisplayName": "Removed Reason"
      },
      {
        "FieldName": "OrganisationType.Type",
        "DisplayName": "Organisation Type"
      },
      {
        "FieldName": "OrganisationData.CompanyNumber",
        "DisplayName": "Company Number"
      },
      {
        "FieldName": "OrganisationData.CharityNumber",
        "DisplayName": "Charity Number"
      },
      {
        "FieldName": "OrganisationData.ParentCompanyGuarantee",
        "DisplayName": "Parent Company Guarantee"
      },
      {
        "FieldName": "OrganisationData.FinancialTrackRecord",
        "DisplayName": "Financial Track Record"
      },
      {
        "FieldName": "OrganisationData.NonLevyContract",
        "DisplayName": "Non Levy Contract"
      }
    ]
  },
  "OrganisationSearchResultsLimit": "5",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConnectionStrings": {
    "Redis": "localhost:6379",
    "Storage": "UseDevelopmentStorage=true;"
  },
  "AzureWebJobsServiceBus": "<ServiceBusConnectionString>",
  "EnvironmentName": "LOCAL"
}
```
  
Open the solution with Visual Studio, and run the project SFA.DAS.RoATPService.Application.Api, running under process 'SFA.DAS.RoATPService.Application.Api' (not IIS)

## Technologies
* .NetCore 10.0
* NUnit
* Moq
* FluentAssertions


  
