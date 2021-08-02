namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Threading.Tasks;
    using Dapper;
    using SFA.DAS.RoatpService.Data.DapperTypeHandlers;
    using SFA.DAS.RoATPService.Application.Commands;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using SFA.DAS.RoATPService.Domain;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class CreateOrganisationRepository: ICreateOrganisationRepository
    {
        private readonly IDbConnectionHelper _dbConnectionHelper;

        public CreateOrganisationRepository(IDbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<Guid?> CreateOrganisation(CreateOrganisationCommand command)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                var startDate = command.StartDate;
                var organisationId = Guid.NewGuid();
                var createdAt = DateTime.Now;
                var createdBy = command.Username;
                var providerTypeId = command.ProviderTypeId;
                var organisationTypeId = command.OrganisationTypeId;
                var statusId = command.OrganisationStatusId;
                var organisationData = new OrganisationData
                {
                    CompanyNumber = command.CompanyNumber?.ToUpper(),
                    CharityNumber = command.CharityNumber,
                    ParentCompanyGuarantee = command.ParentCompanyGuarantee,
                    FinancialTrackRecord = command.FinancialTrackRecord,
                    NonLevyContract = command.NonLevyContract,
                    StartDate = startDate,
                    SourceIsUKRLP = command.SourceIsUKRLP,
                    ApplicationDeterminedDate = command.ApplicationDeterminedDate
                };

                string sql = $"INSERT INTO [dbo].[Organisations] " +
                             " ([Id] " +
                             ",[CreatedAt] " +
                             ",[CreatedBy] " +
                             ",[StatusId] " +
                             ",[ProviderTypeId] " +
                             ",[OrganisationTypeId] " +
                             ",[UKPRN] " +
                             ",[LegalName] " +
                             ",[TradingName] " +
                             ",[StatusDate] " +
                             ",[OrganisationData]) " +
                             "VALUES " +
                             "(@organisationId, @createdAt, @createdBy, @statusId, @providerTypeId, @organisationTypeId," +
                             " @ukprn, @legalName, @tradingName, @statusDate, @organisationData)";

                var organisationsCreated = await connection.ExecuteAsync(sql,
                    new
                    {
                        organisationId,
                        createdAt,
                        createdBy,
                        statusId,
                        providerTypeId,
                        organisationTypeId,
                        command.Ukprn,
                        command.LegalName,
                        command.TradingName,
                        command.StatusDate,
                        organisationData
                    });

                if (organisationsCreated > 0)
                    return organisationId;

                return null;
            }
        }

    }
}
