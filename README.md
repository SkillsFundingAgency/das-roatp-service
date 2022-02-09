# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  Register of Apprenticeship Training Providers (RoATP) API

> **Note:**  
> das-roatp-service repo and contained apps are being deprecated in favour of das-roatp-api 

### Developer Setup

#### Requirements

- Install [Visual Studio 2017 Enterprise](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- Install [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- Install [Azure Storage Emulator](https://go.microsoft.com/fwlink/?linkid=717179&clcid=0x409) (Make sure you are on v5.3 or above)
- Install [Azure Storage Explorer](http://storageexplorer.com/)
- Administrator Access

#### Setup

- Create a Configuration table in your (Development) local storage account.
- Obtain the local config json from the das-employer-config repo (<https://github.com/SkillsFundingAgency/das-employer-config>) and adjust the SqlConnectionString property to match your local setup
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.RoATPService_1.0
  - Data: {The contents of the local config json file}

##### Open the solution

- Open Visual studio as an administrator
- Open the solution
- Set SFA.DAS.RoATPService.Application.Api as the startup project
- Publish the database project SFA.DAS.RoATPService.Database to your local SQL Server instance
- Running the solution will launch the API in your browser



